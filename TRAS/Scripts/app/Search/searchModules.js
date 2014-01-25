var searchManager = (function () {
    function pagination(id, array) {
        $(id).click(function (e) {
            $('#items').hide();

            var l = array.length / 10;
            $('#paginator').bootpag({
                total: l,
                page: 1,
                maxVisible: 10
            }).on("page", function (e, num) {
                $("#entries").empty();
                var max = num * 10;
                var min = max - 10;
                for (var a = min; a < max; a++) {
                    $('#entries').append("<li class=\"thumbnail\"><h4>" + array[a].Name + "</h4></br>" + array[a].FcodeName + "</li>");
                    if (min < array.length - 10)
                        min = min + 10;
                }
            });
        });
    }
    return {
        pagination:pagination
    }
})();
        