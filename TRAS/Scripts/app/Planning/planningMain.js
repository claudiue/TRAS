
$(document).ready(function () {

    mapManager.showMap();

    $("#listItems").sortable({
        handle: '.handle',
        update: function () {
            var order = $('#listItems').sortable('serialize');
            listManager.reorder();
        }
    });

    $("#addItemBtn").click(function () {

        jqManager.sendItem(listManager.getItem());
        listManager.addItem();

    });

    $("#removeItemBtn").click(function () {
        //listManager.removeItem();
    });

    $("#showMap").click(function () {
        mapManager.showMap();
    });

    $("#drawRoute").click(function () {
        mapManager.drawPath();
    });

    $("#geolocateMe").click(function () {
        mapManager.geolocateMe();
    });

});