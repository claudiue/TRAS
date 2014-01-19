(function () {

    testHub = $.connection.testHub;
    $.connection.logging = true;
    $.connection.hub.start();

    testHub.client.newMessage = function (message) {
        model.addMessage(message);
    };

    var Model = function () {
        var self = this;
        self.message = ko.observable("");
        self.messages = ko.observableArray();
    }

    Model.prototype = {
        sendMessage: function () {
            var self = this;
            testHub.server.sendMessage(self.message());
            self.message("");
        },
        addMessage: function (message) {
            var self = this;
            self.messages.push(message);
        }
    };

    var model = new Model();

    $(function () {
        ko.applyBindings(model);
    });

}());