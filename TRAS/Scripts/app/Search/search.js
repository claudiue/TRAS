$(document).ready(function () {
    $('#searchForm').submit(function (event) {
        event.preventDefault();
        var item = $('#search').val();
        $('#content').hide();
        $('#main').append('<ul id =\"items\"></ul>');
        $('#items').show();
        $('#items').empty();
        $('#entries').empty();
        $('#entries').hide();
        $('#paginator').hide();
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
                $("#main").append('<ul id=\"entries\"></ul>');
                $('#main').append('<div id=\"paginator\"></div>');
                if (attractions.length != 0) 
                $('#items').append("<li class=\"thumbnail\" id=\"attr\"><p>Attractions(" + attractions.length + ")</p></li>");
                if(accomodation.length!=0)
                    $('#items').append("<li class=\"thumbnail\" id=\"acc\"><p>Accomodation(" + accomodation.length + ")</p></li>");
                if(restaurants.length!=0)
                    $('#items').append("<li class=\"thumbnail\" id=\"rest\"><p>Restaurants(" + restaurants.length + ")</p></li>");
                if(transportation.length!=0)
                    $('#items').append("<li class=\"thumbnail\" id=\"transp\"><p>Transportation(" + transportation.length + ")</p></li>");
                if(entertaining.length!=0)
                    $('#items').append("<li class=\"thumbnail\" id=\"ent\"><p>Entertainment("+entertaining.length+")</p></li>");
                $('#return').click(function (e) { $("#entries").hide(); $("#items").visible();})
                searchManager.pagination("#attr", attractions);
                searchManager.pagination("#acc", accomodation);
                searchManager.pagination("#rest", restaurants);
                searchManager.pagination("#transp", transportation);
                searchManager.pagination("#ent", entertaining);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var resp = JSON.parse(jqXHR.responseText);
                alert(errorThrown);
            }
        });
       
      
    });

});