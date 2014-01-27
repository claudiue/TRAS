$(document).ready(function () {
    $('#items').hide();
    $('#entry-list').hide();
    $('#searchForm').submit(function (event) {
        $("#mainImg").hide();
        $("#entry-list").show();
        $("#submitBtn").prop('disabled', true);
        event.preventDefault();
        var item = $('#search').val();
        $('#items').show();
        $('#items').empty();

        $('#entries').empty();
        $.ajax({
            type: "POST",
            url: '../Home/Search/',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ query: item }),
            success: function (data, textStatus, jqXHR) {
                var accomodation = new Array();
                var attractions = new Array();
                var restaurants = new Array();
                var transportation = new Array();
                var entertaining = new Array();
            
                for (var i = 0; i < data.spots.length; i++) {
                    if(data.spots[i].Fcode == "CMP" || data.spots[i].Fcode == "HTL")
                        accomodation.push(data.spots[i]);
                    else if (data.spots[i].Fcode=="MNMT" || data.spots[i].Fcode == "MUS" ||  data.spots[i].Fcode == "ARCH" 
                        ||  data.spots[i].Fcode == "CAVE" ||  data.spots[i].Fcode == "CH" ||  data.spots[i].Fcode == "CSTL" 
                        ||  data.spots[i].Fcode == "MSQE" ||  data.spots[i].Fcode == "MSTY" ||  data.spots[i].Fcode == "PAL"
                        ||  data.spots[i].Fcode == "PYR" || data.spots[i].Fcode == "PYRS" ||  data.spots[i].Fcode == "SQR"
                        ||  data.spots[i].Fcode == "TOWR" ||  data.spots[i].Fcode == "ZOO"||  data.spots[i].Fcode == "HSTS")
                        attractions.push(data.spots[i]);
                    else if (data.spots[i].Fcode == "REST")
                        restaurants.push(data.spots[i]);
                    else if (data.spots[i].Fcode=="AIRP" || data.spots[i].Fcode == "BUSTP" || data.spots[i].Fcode == "FY"
                        ||  data.spots[i].Fcode == "MTRO" ||  data.spots[i].Fcode == "RSTN")
                        transportation.push(data.spots[i]);
                    else if (data.spots[i].Fcode == "CSNO" ||  data.spots[i].Fcode == "GDN" || data.spots[i].Fcode == "MALL"
                        ||  data.spots[i].Fcode == "RSRT" ||  data.spots[i].Fcode == "SPA" ||  data.spots[i].Fcode == "STDM"
                        ||  data.spots[i].Fcode == "THTR" ||  data.spots[i].Fcode == "RSRT")
                        entertaining.push(data.spots[i]);
                }
                if (attractions.length != 0) 
                    $('#items').append("<li id=\"attr\" class=\"active\"><strong>Attractions (" + attractions.length + ")</strong><i class=\"glyphicon glyphicon-chevron-right grey pull-right\"></i></li>");
                if(accomodation.length!=0)
                    $('#items').append("<li id=\"acc\"><strong>Accomodation (" + accomodation.length + ")</strong><i class=\"glyphicon glyphicon-chevron-right pull-right\"></i></li>");
                if(restaurants.length!=0)
                    $('#items').append("<li id=\"rest\"><strong>Restaurants (" + restaurants.length + ")</strong><i class=\"glyphicon glyphicon-chevron-right pull-right\"></i></li>");
                if(transportation.length!=0)
                    $('#items').append("<li id=\"transp\"><strong>Transportation (" + transportation.length + ")</strong><i class=\"glyphicon glyphicon-chevron-right pull-right\"></i></li>");
                if(entertaining.length!=0)
                    $('#items').append("<li id=\"ent\"><strong>Entertainment (" + entertaining.length + ")</strong><i class=\"glyphicon glyphicon-chevron-right pull-right\"></i></li>");
                $('#return').click(function (e) { $("#entries").hide(); $("#items").visible(); });

                $("#submitBtn").prop('disabled', false);
                $("#attr").on("click", function (e) { searchManager.pagination("#attr", attractions) });
                $("#acc").on("click",function(e){searchManager.pagination("#acc", accomodation)});
                $("#rest").on("click",function(e){searchManager.pagination("#rest", restaurants)});
                $("#transp").on("click",function(e){searchManager.pagination("#transp", transportation)});
                $("#ent").on("click", function (e) { searchManager.pagination("#ent", entertaining) });
                $("#attr").trigger("click");

            },
            error: function (jqXHR, textStatus, errorThrown) {
                var resp = JSON.parse(jqXHR.responseText);
                alert(errorThrown);
            }
        });
        $("#attr").trigger("click");
       

    });

});