
$(document).ready(function () {

    $("#listItems").sortable({
        handle: '.handle',
        update: function () {
            var order = $('#listItems').sortable('serialize');
            //listManager.order();
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

});