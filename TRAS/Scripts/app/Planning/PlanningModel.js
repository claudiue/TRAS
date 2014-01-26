
function PlanningModel() {

    var self = this;

    // locations list + map
    self.query = ko.observable("");
    self.items = ko.observableArray([]);
    self.newItemText = ko.observable();
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
    self.features = ko.observableArray([]);


    var map,
        flightPath,
        line = [],
        locations = [],
        infos = [],
        LatLngList = [],
        bounds = new google.maps.LatLngBounds(),
        markersArray = [],
        markersSpotsArray = [],
        locs = [];

    // Operations
    self.addItem = function () {

        self.items.push({ query: this.newItemText() });

        var item = ko.toJSON({ query: this.newItemText() });
        self.sendItem(item);

        locations.push(jQuery.parseJSON(item).query);

        self.newItemText("");

    };

    self.createPath = function (item) {
        console.log(self.places());
        

        //if (self.places().length > 1) {
        //    self.drawPath();
        //} else {

        //}

        return true;
    }

    self.removeItem = function (item) {

        self.items.remove(item);

        var item = jQuery.parseJSON(ko.toJSON(item)).query;
        self.places.remove(item);
        
        remove_item(locations, item);
        remove_item(infos, item);
        removeMarker(item);

        self.drawPath();
    };

    remove_item = function (arr, value) {
        for (b in arr) {
            if (arr[b] == value || arr[b].loc == value) {
                arr.splice(b, 1);
                break;
            }
        }
        return arr;
    }

    self.sendItem = function (item) {

        $.ajax({
            type: "POST",
            url: '../Planning/Search/',
            contentType: "application/json; charset=utf-8",
            data: item,
            success: function (data, textStatus, jqXHR) {

                var it = jQuery.parseJSON(item).query;

                infos.push({ loc: it, data: data });

                self.showMarker(it, data);
                self.addToponym(it, data);

            },
            error: function (jqXHR, textStatus, errorThrown) {
                var resp = JSON.parse(jqXHR.responseText);
                alert(errorThrown);
            }
        });
    }

    self.showMarker = function (location, data) {

        var myLatlng = new google.maps.LatLng(data.Latitude, data.Longitude);

        var marker = new google.maps.Marker({
            position: myLatlng,
            map: map,
            loc: location
        });
        markersArray.push({ value: location, marker: marker });

        map.setZoom(10);
        map.setCenter(myLatlng);

        //LatLngList.push(myLatlng);
        self.zoomAtClick(marker);
    }

    removeMarker = function (item) {
        for (var i = 0; i < markersArray.length; i++) {
            if (markersArray[i].value == item) {
                markersArray[i].marker.setMap(null);
                markersArray.splice(i, 1);
                LatLngList.splice(i, 1);
            }

        }
        //for (var i = 0; i < markersArray.length; i++) {
        //    console.log(markersArray);
        //    console.log(LatLngList);
        //}
    }

    //?
    self.addToponym = function (location, description) {

        self.toponymsList.push({
            index: self.toponymsList.length,
            key: location,
            value: description
        });
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
    }

    self.geolocateMe = function () {

        // Try HTML5 geolocation
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (position) {
                var pos = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);

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

        var list = self.places(),//locations,
            flightPlanCoordinates = [],
            geocoder = new google.maps.Geocoder();

        for (i = 0; i < line.length; i++) {
            line[i].setMap(null); //line[i].setVisible(false);
        }

        var path = [];

        for (var i = 0; i < self.places().length; i++) {
            for (var j = 0; j < infos.length; j++) {

                if (self.places()[i] == infos[j].loc) {
                    path.push({
                        name: self.places()[i],
                        lat: infos[i].data.Latitude,
                        lng: infos[i].data.Longitude
                    });
                    break;
                }
            }
            flightPlanCoordinates.push(new google.maps.LatLng(path[i].lat, path[i].lng));
            LatLngList.push(new google.maps.LatLng(path[i].lat, path[i].lng));
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
        for (var i = 0; i < markersArray.length; i++) {
            markersArray[i].setMap(null);
        }

        for (var i = 0; i < markersSpotsArray.length; i++) {
            markersSpotsArray[i].setMap(null);
        }

        markersSpotsArray = [];
        markersArray = [];
    }

    extendBounds = function () {
        for (var i = 0; i < LatLngList.length; i++) {
            map.setZoom(15);
            bounds.extend(LatLngList[i]);
        }
        map.fitBounds(bounds);
    }

    //?
    self.zoomAtClick = function (marker) {
        google.maps.event.addListener(marker, 'click', function () {
            map.setZoom(12);
            map.setCenter(marker.getPosition());
            self.searchSpots(marker.loc);
        });
    }

    self.searchSpots = function (item) {

        
        $.ajax({
            type: "POST",
            url: '../Planning/SearchSpots/',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ query: item }),
            success: function (data, textStatus, jqXHR) {

                document.getElementById("spotsList").innerHTML = "";

                console.log(data);
                data = data.spots;

                for (var i = 0; i < data.length; i++) {
                    console.log(data[i]);
                    self.spots.push({ spot: data[i].Name, lat: data[i].Lat, lng: data[i].Lng });
                    locs.push({ name: data[i].Name, lat: data[i].Lat, lng: data[i].Lng });

                    var latlng = new google.maps.LatLng(data[i].Lat, data[i].Lng);

                    var marker = new google.maps.Marker({
                        position: latlng,
                        map: map,
                        title: data[i].Name
                    });
                    markersSpotsArray.push({ value: data[i].Name, marker: marker });
                }
                console.log(locs);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var resp = JSON.parse(jqXHR.responseText);
                alert(errorThrown);
            }
        });
    }

    showOnMap = function (spot) {
        for (var i = 0; i < locs.length; i++) {
            if (locs[i].name == spot) {
                var lat = locs[i].lat;
                var lng = locs[i].lng;

                map.setCenter(new google.maps.LatLng(lat, lng));
                map.setZoom(17);
            }
        }
        console.log(spot);
    }

    self.saveItinerary = function () {
        console.log(self.itineraryName());
    }
}