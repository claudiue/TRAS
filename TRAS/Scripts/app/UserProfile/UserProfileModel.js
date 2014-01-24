
function appViewModel() {
    var self = this;

    self.firstName = ko.observable();
    self.lastName = ko.observable();
    self.age = ko.observable();
    self.location = ko.observable();
    self.genderValue = ko.observable("male");

    self.sendData = function () {
        var data = {
            firstName: self.firstName(),
            lastName: self.lastName(),
            gender: self.genderValue(),
            age: self.age(),
            location: self.location()
        };

        alert(JSON.stringify(data));

        $.ajax({
            type: "POST",
            url: '../UserProfile/UpdateProfile/',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify( data ),
            success: function (data, textStatus, jqXHR) {

                //console.log(data);
                
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var resp = JSON.parse(jqXHR.responseText);
                alert(errorThrown);
            }
        });
    }

    self.performClick = function(node) {
        var evt = document.createEvent("MouseEvents");
        evt.initEvent("click", true, false);
        node.dispatchEvent(evt);
    }


    //$(":file").change(function () {
    //    alert($(":file").val());
    //});
}

// Activates knockout.js
ko.applyBindings(new appViewModel());