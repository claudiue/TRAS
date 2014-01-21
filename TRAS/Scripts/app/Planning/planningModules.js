
var listManager = (function () {

    var itemsList = [];

    function getItem() {
        return $("#itemValue").val();
    }

    function clearInput(input) {
        $(input).val("");
    }

    function createList(){

        var list = document.getElementById("listItems"),
            elem = document.createElement("li"),
            img = document.createElement('img');

        var //item = document.createElement("div"),
            text = document.createElement("p"),
            removeBtn = document.createElement("button");

        img.setAttribute('src', '../../Content/images/move.jpg');
        img.setAttribute('class', 'handle');

        var itemValue = getItem();
        text.setAttribute('class', '');
        text.innerText = itemValue;
        itemsList.push(itemValue);

        removeBtn.innerText = 'x';
        //removeBtn.setAttribute("id", "removeItemBtn"); ??unique

        elem.setAttribute('class', 'item');
        elem.appendChild(img);
        elem.appendChild(text);
        
        //elem.appendChild(item);
        //elem.appendChild(img);
        list.appendChild(elem);

        $("#divItems").append(list);
        clearInput("#itemValue");
    }

    function addItem() {
        if (getItem() != "") {
            createList();
        }
    }

    function reorder() {
        //$(".item").each(function (index) {
        //    console.log(index + ": " + $(this).text());
        //});
    }
    return {
        itemsList: itemsList,
        addItem: addItem,
        getItem: getItem,
        reorder: reorder
    }
})();

var mapManager = (function () {

    var map;
    var flightPath;

    
    function showMap() {

        if (flightPath !== undefined) {
            flightPath.setMap(null); // remove the line
        }

        var mapOptions = {
            zoom: 8,
            center: new google.maps.LatLng(-34.397, 150.644)
        };

        map = new google.maps.Map(document.getElementById('map'), mapOptions);      
    }

    function geolocateMe() {

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


    function drawPath() {

        var list = listManager.itemsList,
            flightPlanCoordinates = [],
            geocoder = new google.maps.Geocoder();

        console.log(list);
        for (var i = 0; i < list.length; i++) {
            var address = list[i];

            geocoder.geocode({ 'address': address }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {

                    map.setCenter(results[0].geometry.location);

                    //console.log(results[0].geometry.location.lat());
                    var lat = results[0].geometry.location.lat();
                    var lng = results[0].geometry.location.lng();

                    flightPlanCoordinates.push(new google.maps.LatLng(lat, lng));
                    
                } else {
                    alert("Geocode was not successful for the following reason: " + status);
                }
            });

        }

        setTimeout(function () {
            flightPath = new google.maps.Polyline({
                path: flightPlanCoordinates,
                geodesic: true,
                strokeColor: 'green',
                strokeOpacity: 1.0,
                strokeWeight: 4
            });

            flightPath.setMap(map);
        }, 300);
              
    }

    return {
        map: map,
        geolocateMe: geolocateMe,
        showMap: showMap,
        drawPath: drawPath
    }
})();

var jqManager = (function () {

    
    function sendItem(item) {

        $.ajax({
            type: "POST",
            url: '../Planning/Search/',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ query: item }),
            success: function (data, textStatus, jqXHR) {

                //console.log(data);
                toponymsManager.addToponym(item, data);
                
                //$('#feature').html(JSON.stringify(data, null, 4));


            },
            error: function (jqXHR, textStatus, errorThrown) {
                var resp = JSON.parse(jqXHR.responseText);
                alert(errorThrown);
            }
        });
    }

    return {
        sendItem: sendItem
    }
})();

var toponymsManager = (function () {
    var toponymsList = [];

    function getToponymsList() {
        return toponymsList;
    }

    function addToponym( location, description ) {
        toponymsList.push({
            index: toponymsList.length,
            key: location,
            value: description
        });

        console.log(toponymsList);
    }

    return {
        toponymsList: getToponymsList,
        addToponym: addToponym
    }

})();