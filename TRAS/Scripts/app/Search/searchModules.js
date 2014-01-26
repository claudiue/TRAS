var searchManager = (function () {
    function pagination(id, array) {
            $(id).addClass('active');
            $(id).siblings().each(function (index) {
                $(this).removeClass('active');
            }
   );
            var maxim=0;
            $('#entries').empty();
            if (array.length < 10)
                maxim = array.length;
            else
                maxim = 10;
              for (var a = 0; a < maxim; a++)
                $('#entries').append("<li class=\"thumbnail\"><h4>" + array[a].Name + "</h4></li>");
             var l = array.length / 10+1;
            $('#paginator').bootpag({
                total: l,
                page: 1
            }).on("page", function (e, num) {
                $("#entries").empty();
                var max =0;
                if (max < array.length)
                    max = num * 10;
                else
                    max = array.length;
                var min = max - 10;
                 for (var a = min; a < max; a++) {
                    $('#entries').append("<li class=\"thumbnail\"><h4>" + array[a].Name + "</h4></li>");
                    //console.log(a);
                    if (min < array.length - 10)
                        min = min + 10;
                }
            });
        }
    
    return {
        pagination:pagination
    }
})();


        