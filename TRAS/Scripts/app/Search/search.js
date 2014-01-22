$(document).ready(function () {
    $('#searchForm').submit(function (event) {
        event.preventDefault(); 
        var item = $('#search').val();
        $('#content').hide();
        $.ajax({
            type: "POST",
            url: '../Home/Search/',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ query: item }),
            success: function (data, textStatus, jqXHR) {
             console.log(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var resp = JSON.parse(jqXHR.responseText);
                alert(errorThrown);
            }
        });

      
    });

});