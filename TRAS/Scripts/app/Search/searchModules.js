var searchManager = {
    pagination: function (id, array) {
        $(id).addClass('active');
        $(id).siblings().each(function (index) {
            $(this).removeClass('active');
        });
        var maxim = 0;
        $('#entries').empty();
        if (array.length < 10)
            maxim = array.length;
        else
            maxim = 10;
        for (var a = 0; a < maxim; a++)
            $('#entries').append("<li class=\"thumbnail\" id=\"test\" onclick=\"location.href=\'#details\'\"><h4>" + array[a].Name + "</h4></li>");
       
        var l = array.length%10==0? array.length / 10:array.length / 10 + 1;
        $('#paginator').bootpag({
            maxVisible: 10,
            total: l,
            page: 1
        }).on("page", function (e, num) {
            $("#entries").empty();
            var max = 0;
            if (array.length < num * 10) {
                max = array.length;
            } else {
                max = num * 10;
            }
            var min = 10*(num-1);
            var content = "";
            for (var a = min; a < max; a++) {
                debugger;
                content += "<li class=\"thumbnail\"><h4>" + array[a].Name + "</h4></li>";
                //console.log(a);
                if (min < array.length - 10)
                    min = min + 10;
                
            }
            $('#entries').html(content);
            $("#entries li").on("click", function (e) {
                var item = $(this).text();
                detailsManager.getDetails(item);

               
            });
        });
        $("#entries li").on("click", function (e) {
            var item = $(this).text();
            detailsManager.getDetails(item);

        });
    }
};      
