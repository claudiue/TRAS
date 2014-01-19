
$(document).ready(function () {
    $("#listItems").sortable({
        handle: '.handle',
        update: function () {
            var order = $('#listItems').sortable('serialize');
            //$("#info").load("process-sortable.php?" + order);
        }
    });
});

var module = (function () {

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

        var item = document.createElement("div"),
            text = document.createElement("p"),
            removeBtn = document.createElement("button");

        img.setAttribute('src', '../../Content/images/move.jpg');
        img.setAttribute('class', 'handle');

        var itemValue = getItem();
        text.innerText = itemValue;
        itemsList.push(itemValue);

        removeBtn.innerText = 'x';
        //removeBtn.setAttribute("id", "removeItemBtn"); ??unique

        item.appendChild(text);
        item.appendChild(img);
        elem.appendChild(item);
        //elem.appendChild(img);
        list.appendChild(elem);

        $("#divItems").append(list);
        clearInput("#itemValue");
    }

    //$(document).ready(function () {
    //    $("#listItems").sortable({
    //        handle: '.handle',
    //        update: function () {
    //            var order = $('#listItems').sortable('serialize');
    //            //$("#info").load("process-sortable.php?" + order);
    //        }
    //    });
    //});


    function addItem() {
        if (getItem() != "") {
            createList();
        }
    }

    return {
        itemsList: itemsList,
        addItem: addItem
    }
})();

var mapModule = (function () {

    var map;

    function showMap() {

        //$("p").hide();

        var mapOptions = {
            zoom: 8,
            center: new google.maps.LatLng(-34.397, 150.644)
        };

        map = new google.maps.Map(document.getElementById('map'),
            mapOptions);
    }

    function drawLine() {

        var list = module.itemsList,
            flightPlanCoordinates = [],
            geocoder = new google.maps.Geocoder(),
            flightPath;

        console.log(list);
        //flightPath.setMap(null); // remove the line
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
        showMap: showMap,
        drawLine: drawLine
    }
})();

$("#addItemBtn").click(function () {
    module.addItem();
});

$("#removeItemBtn").click(function () {
    //module.removeItem();
});

$("#showMap").click(function () {
    mapModule.showMap();
});

$("#drawRoute").click(function () {
    mapModule.drawLine();
});


