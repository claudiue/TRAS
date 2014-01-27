
function PlanningModel() {

    var self = this;

    self.locations = ko.observableArray([]);

    self.photoValue = ko.observable();
    self.images = ko.observableArray([]);

    // locations list + map
    self.query = ko.observable("");
    self.queryValue = ko.observable();
    self.spots = ko.observableArray([]);
    self.toponymsList = [];

    // itinerary
    self.itineraryName = ko.observable("");
    self.budget = ko.observable(0);
    self.nrDays = ko.observable(0);
    self.rating = ko.observable(0);
    self.startDate = ko.observable();
    self.endDate = ko.observable();
    self.places = ko.observableArray([]);

    // location
    self.locationName = ko.observable("");
    self.latitude = ko.observable();
    self.longitude = ko.observable();
    self.allFeatures = ko.observableArray([]);

    self.obj = [
        {
            val: "Iasi",
            items: ["Gradina Botanica", "Palatul Culturii"]
        },
        {
            val: "Vaslui",
            items: ["Copou", "Centru"]
        }
        ];

    self.itinerary = {
        name: 'MyItinerary',
        rating: '10',
        places: [
            {
                data: {
                    id: '1',
                    name: 'Iasi',
                    lat: '12',
                    lng: '23'
                },
                features: [
                    {
                        data: {
                            id: '11',
                            lat: '47',
                            lng: '25'
                        }
                    },
                    {
                        data: {
                            id: '111',
                            lat: '27',
                            lng: '15'
                        }
                    }
                ]
            },
            {
                name: 'Vaslui',
                data: {
                    id: '2',
                    lat: '37',
                    lng: '45'
                }
            }
        ]
    };

    self.itineraryPlaces = ko.observableArray([]);
   

    var map,
        flightPath,
        line = [],
        bounds = new google.maps.LatLngBounds(),
        markersArray = [],
        markersSpotsArray = [],
        locs = [],
        path = [];

    // Operations
    self.addItem = function () {

        self.sendItem(ko.toJSON({ query: self.queryValue() }));
        self.queryValue("");

    };

    self.sendItem = function (item) {
        $.ajax({
            type: "POST",
            url: '../Planning/Search/',
            contentType: "application/json; charset=utf-8",
            data: item,
            success: function (data, textStatus, jqXHR) {

                //console.log(data);

                showMarker(data.Name, data.Latitude, data.Longitude);
                self.locations.push({ locationData: data, features: [] });
                self.checkPlace(item);
                
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var resp = JSON.parse(jqXHR.responseText);
                alert(errorThrown);
            }
        });
    };

    self.checkPlace = function (item) {
        
        if (self.locations().length > 1) {
            self.drawPath();
        } else {
            for (i = 0; i < line.length; i++) {
                line[i].setMap(null);
                line = [];
            }
            path = [];
        }

        return true;
    }

    self.removeItem = function (item) {

        document.getElementById("spotsList").innerHTML = "";

        self.locations.remove(item);
        item.features = [];
        self.allFeatures = [];


        var itemValue = item.locationData.Name;//jQuery.parseJSON(ko.toJSON(item)).query;
        //self.places.remove(itemValue);
        
        //remove_item(self.locations, itemValue);
        removeMarker(item);

        self.drawPath();
    };

    remove_item = function (arr, value) {
        for (b in arr) {
            if (arr[b] == value) {
                arr.splice(b, 1);
                break;
            }
        }
        return arr;
    }
    markersSpotsArray = [];
    showSpotMarker = function (parent, name, lat, lng) {

        //console.log(parent);
        //console.log(name);
        
        var myLatlng = new google.maps.LatLng(lat, lng);

        var marker = new google.maps.Marker({
            position: myLatlng,
            map: map,
            loc: name
        });
        markersSpotsArray.push({ parent: parent, value: name, marker: marker });
        //console.log(markersSpotsArray);
        //map.setZoom(10);
        //map.setCenter(myLatlng);

        self.zoomAtClick(marker);
    }

    showMarker = function (name, lat, lng) {

        var myLatlng = new google.maps.LatLng(lat, lng);

        var marker = new google.maps.Marker({
            position: myLatlng,
            map: map,
            loc: name
        });
        markersArray.push({ value: name, marker: marker });

        map.setZoom(10);
        map.setCenter(myLatlng);

        //self.zoomAtClick(marker);
    }

    removeMarker = function (item) {

        var itemValue = item.locationData.Name;
        var parent = item.locationData.AdminName1;

        if (markersSpotsArray.length > 0) {
            for (var i = 0; i < markersArray.length; i++) {
                //console.log(markersSpotsArray[i]);
                if (markersSpotsArray[i].value == itemValue && markersSpotsArray[i].parent == parent) {
                    markersSpotsArray[i].marker.setMap(null);
                    markersSpotsArray.splice(i, 1);
                    //path.splice(i, 1);
                }
            }
        }

        for (var i = 0; i < markersArray.length; i++) {
            if (markersArray[i].value == itemValue) {
                markersArray[i].marker.setMap(null);
                markersArray.splice(i, 1);
                path.splice(i, 1);
            }

        }
    }

    self.showMap = function () {

        
        if (flightPath !== undefined) {
            flightPath.setMap(null); // remove the line
        }

        var mapOptions = {
            zoom: 8,
            center: new google.maps.LatLng(-34.397, 150.644)
        };

        map = new google.maps.Map(document.getElementById('map'), mapOptions);

       
        var clearMapControl = document.createElement('button');
        var t = document.createTextNode("Clear map");
        clearMapControl.appendChild(t);
        google.maps.event.addDomListener(clearMapControl, 'click', function () {
            clearMarkers();
        });

        var geolocateMeControl = document.createElement('button');
        var geoTitle = document.createTextNode("?");
        geolocateMeControl.appendChild(geoTitle);
        google.maps.event.addDomListener(geolocateMeControl, 'click', function () {
            self.geolocateMe();
        });

        clearMapControl.index = 1;
        map.controls[google.maps.ControlPosition.TOP_RIGHT].push(clearMapControl);
        geolocateMeControl.index = 1;
        map.controls[google.maps.ControlPosition.TOP_RIGHT].push(geolocateMeControl);

    }

    self.geolocateMe = function () {

        // Try HTML5 geolocation
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (position) {
                
                var pos = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);

                var marker = new google.maps.Marker({
                    position: pos,
                    map: map,
                    loc: name
                });
                var infowindow = new google.maps.InfoWindow({
                    map: map,
                    position: pos,
                    content: 'You are here!'
                });


                map.setCenter(pos);
            });
        } else {
            // Browser doesn't support Geolocation
            alert("Browser doesn't support Geolocation!");
        }
    }

    self.drawPath = function () {

        var flightPlanCoordinates = [],
            geocoder = new google.maps.Geocoder();

        for (i = 0; i < line.length; i++) {
            line[i].setMap(null); //line[i].setVisible(false);
            line = [];
        }

        path = [];
        //console.log(self.locations());
       // for (var i = 0; i < self.places().length; i++) {
        for (var j = 0; j < self.locations().length; j++) {
             //   if (self.places()[i] == self.locations[j].locationData.Name) {
                    path.push({
                        name: self.locations()[j].locationData.Name,
                        lat: self.locations()[j].locationData.Latitude,
                        lng: self.locations()[j].locationData.Longitude
                    });
                //    break;
                }
            //}
        //}

        for (var i = 0; i < path.length; i++) {
            flightPlanCoordinates.push(new google.maps.LatLng(path[i].lat, path[i].lng));
        }
        extendBounds();

        setTimeout(function () {
            flightPath = new google.maps.Polyline({
                path: flightPlanCoordinates,
                geodesic: true,
                strokeColor: 'green',
                strokeOpacity: 1.0,
                strokeWeight: 2
            });

            line.push(flightPath);
            flightPath.setMap(map);
        }, 200);
    }

    clearMarkers = function () {

        console.log(markersArray);

        for (var i = 0; i < markersArray.length; i++) {
            markersArray[i].marker.setMap(null);
        }

        for (var i = 0; i < markersSpotsArray.length; i++) {
            markersSpotsArray[i].marker.setMap(null);
        }

        markersSpotsArray = [];
        markersArray = [];
        path = [];
    }

    extendBounds = function () {
        for (var i = 0; i < path.length; i++) {
            map.setZoom(15);
            bounds.extend(new google.maps.LatLng(path[i].lat, path[i].lng));
        }
        map.fitBounds(bounds);
    }

    self.zoomAtClick = function (marker) {
        google.maps.event.addListener(marker, 'click', function () {
            map.setZoom(12);
            map.setCenter(marker.getPosition());
            self.searchSpots(marker.loc);
        });
    }

    self.searchSpots = function (item) {
        //console.log(item.Name);

        var id = item.GeoNameId;
        var item = item.Name;

        $.ajax({
            type: "POST",
            url: '../Planning/SearchSpots/',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ query: item }),
            success: function (data, textStatus, jqXHR) {

                document.getElementById("spotsList").innerHTML = "";
                data = data.spots;
                
                for (var i = 0; i < data.length; i++) {

                    self.spots.push({ parentId: id, spotData: data[i] });
                    //var latlng = new google.maps.LatLng(data[i].Lat, data[i].Lng);

                    //var marker = new google.maps.Marker({
                    //    position: latlng,
                    //    map: map,
                    //    title: data[i].Name
                    //});
                    //markersSpotsArray.push({ value: data[i].Name, marker: marker });
                }
                //console.log(locs);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var resp = JSON.parse(jqXHR.responseText);
                alert(errorThrown);
            }
        });
    }

    showOnMap = function (spot) {
        for (var i = 0; i < self.spots().length; i++) {

            var parent = self.spots()[i].spotData.AdminName1;
            var name = self.spots()[i].spotData.Name;

            if (name == spot) {
                var lat = self.spots()[i].spotData.Lat;
                var lng = self.spots()[i].spotData.Lng;

                showSpotMarker(parent, name, lat, lng);
                //map.setCenter(new google.maps.LatLng(lat, lng));
                //map.setZoom(13);
            }
        }
    }

    self.saveItinerary = function () {

        var itinerary = {
            name: self.itineraryName(),
            budget: self.budget(),
            days: self.nrDays(),
            locations: self.locations(),
            rating: self.rating()
        };

        //console.log(itinerary);

        $.ajax({
            type: "POST",
            url: '../Planning/SaveItinerary/',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(itinerary),
            success: function (data, textStatus, jqXHR) {

                //console.log(data);
                $(location).attr('href', "/Itineraries/UserItineraries/");

            },
            error: function (jqXHR, textStatus, errorThrown) {
                var resp = JSON.parse(jqXHR.responseText);
                alert(errorThrown);
            }
        });

    }

    self.addToItinerary = function (item) {
        //console.log(self.allFeatures());
        for (var i = 0; i < self.locations().length; i++) {
            if (item.parentId == self.locations()[i].locationData.GeoNameId) {
                self.locations()[i].features.push({ data: item });
            }
        }

        //self.features().splice(0, self.features().length);

        //console.log(self.locations());
        //console.log(self.locations()[0].locationData.Name);
        //console.log(self.locations()[0].features[0].data.spotData.Name);

        return true;
    }

    

    self.searchPhotos = function () {

        //console.log(self.photoValue());

        data = [
                    { URI: "http://dalepollak.com/wp-content/uploads/2013/12/bmw_i8_spyder_concept_car.jpg", Title: "" },
                    { URI: "http://dalepollak.com/wp-content/uploads/2013/12/bmw_i8_spyder_concept_car.jpg", Title: "" }
        ];

        for (var i = 0; i < data.length; i++) {
            self.images.push({ URI: data[0].URI, Title: data[0].Title });
        }

        //self.images.push({ URI: data[0].URI, Title: data[0].Title });
        console.log(self.images());
        //displayPhotos();

        //$.ajax({
        //    type: "POST",
        //    url: '../Planning/SearchFlickr/',
        //    contentType: "application/json; charset=utf-8",
        //    data: JSON.stringify({ query: item }),
        //    success: function (data, textStatus, jqXHR) {
        //        debugger;
        //        console.log(data);
        //        //data = [
        //        //    { URI: "", Title: "" },
        //        //    { URI: "", Title: "" }
        //        //];
        //        //self.photos().push(data);
        //        //displayPhotos();
        //    },
        //    error: function (jqXHR, textStatus, errorThrown) {
        //        var resp = JSON.parse(jqXHR.responseText);
        //        alert(errorThrown);
        //    }
        //});
    }


    displayPhotos = function () {

    }

}