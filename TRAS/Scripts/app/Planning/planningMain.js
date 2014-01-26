
$(document).ready(function () {

    var model = new PlanningModel();
    ko.applyBindings(model);
    model.showMap();

});