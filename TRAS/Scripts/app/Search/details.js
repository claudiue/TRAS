var detailsManager = {
    getDetails: function (itemName) {
        $('#details').empty();
        $('#details').show();
        var playListURL = 'http://en.wikipedia.org/w/api.php?format=json&action=query&titles=' + itemName + '&prop=revisions&rvprop=content&&rvparse&callback=?';
        $.getJSON(playListURL, function (data) {

            for (var pageId in data.query.pages) {
                if (data.query.pages.hasOwnProperty(pageId)) {
                    $('#entry-list').append('<div id=\'details\'></div>');
                    $('#details').append(data.query.pages[pageId].revisions[0]['*']);
                }
            }

        });

    }
};