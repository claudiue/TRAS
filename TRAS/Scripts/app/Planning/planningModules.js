

function Item(data) {
    this.query = ko.observable(data.query);
}

function ItemListViewModel() {

    var map,
        flightPath,
        line = [],
        locations = [],
        infos = [];

    var self = this;

    self.items = ko.observableArray([]);
    self.newItemText = ko.observable();
    
    // Operations
    self.addItem = function () {

        self.items.push(new Item({ query: this.newItemText() }));

        var item = ko.toJSON(new Item({ query: this.newItemText() }));
        self.sendItem( item );

        locations.push(jQuery.parseJSON(item).query);

        self.newItemText("");
    };

    self.removeItem = function (item) {
        self.items.remove(item);
        remove_item(locations, jQuery.parseJSON(ko.toJSON(item)).query);

        remove_item(infos, jQuery.parseJSON(ko.toJSON(item)).query);
        
        removeMarker(jQuery.parseJSON(ko.toJSON(item)).query);

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

                infos.push({ loc: jQuery.parseJSON(item).query, data: data });
                self.showMarker(jQuery.parseJSON(item).query, data);
                self.addToponym(jQuery.parseJSON(item).query, data);

            },
            error: function (jqXHR, textStatus, errorThrown) {
                var resp = JSON.parse(jqXHR.responseText);
                alert(errorThrown);
            }
        });
    }

    var LatLngList = [];
    var bounds = new google.maps.LatLngBounds();

    self.showMarker = function (location, data) {

        var myLatlng = new google.maps.LatLng(data.Latitude, data.Longitude);
       
        var marker = new google.maps.Marker({
            position: myLatlng,
            map: map,
            title: 'Hello World!'
        });
        markersArray.push({value: location, marker: marker});

        map.setZoom(10);
        map.setCenter(myLatlng);

        LatLngList.push(myLatlng);
        
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

    self.toponymsList = [];

    self.addToponym = function (location, description) {

        self.toponymsList.push({
            index: self.toponymsList.length,
            key: location,
            value: description
        });
    }

    self.showMap = function() {

        if (flightPath !== undefined) {
            flightPath.setMap(null); // remove the line
        }

        var mapOptions = {
            zoom: 8,
            center: new google.maps.LatLng(-34.397, 150.644)
        };

        map = new google.maps.Map(document.getElementById('map'), mapOptions);      
    }

    self.geolocateMe = function() {

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

        var list = locations,
            flightPlanCoordinates = [],
            geocoder = new google.maps.Geocoder();

        for (i = 0; i < line.length; i++) {
            line[i].setMap(null); //line[i].setVisible(false);
        }

        for (var i = 0; i < infos.length; i++) {
            //var address = list[i];

            var lat = infos[i].data.Latitude;
            var lng = infos[i].data.Longitude;

            flightPlanCoordinates.push(new google.maps.LatLng(lat, lng));

            //geocoder.geocode({ 'address': address }, function (results, status) {
            //    if (status == google.maps.GeocoderStatus.OK) {

            //        var lat = results[0].geometry.location.lat();
            //        var lng = results[0].geometry.location.lng();

            //        flightPlanCoordinates.push(new google.maps.LatLng(lat, lng));

            //    } else {
            //        alert("Geocode was not successful for the following reason: " + status);
            //    }
            //});

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

    var markersArray = [];
    clearMarkers = function () {
        for (var i = 0; i < markersArray.length; i++) {
            markersArray[i].setMap(null);
        }

        markersArray = [];
    }

    extendBounds = function () {
        for (var i = 0; i < LatLngList.length; i++) {
            map.setZoom(15);
            bounds.extend(LatLngList[i]);
        }
        map.fitBounds(bounds);
    }
}

var vm = new ItemListViewModel();
ko.applyBindings(vm);

vm.showMap();