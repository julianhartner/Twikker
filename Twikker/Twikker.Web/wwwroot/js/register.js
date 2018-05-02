function RegisterViewModel() {
    var self = this;
    self.Username = ko.observable();
    self.Email = ko.observable("");
    self.Password = ko.observable("");
    self.ConfirmPassword = ko.observable("");
    self.Message = ko.observable();
    self.Class = ko.observable();
    
    self.Register = function () {
        $.ajax({
            type: "POST",
            url: "RegisterUser",
            data: { "username": self.Username(), "email": self.Email(), "password": self.Password(), "confirmPassword": self.ConfirmPassword() },
            dataType: "json",
            success: function (response) {
                if (response !== null) {
                    var parsedResponse = JSON.parse(response);
                    console.log(parsedResponse);
                    self.Message(parsedResponse.description);
                    self.Class(parsedResponse.class);
                }
            },
            failure: function (response) {
                alert("Error while retrieving data!");
            }
        });
    }
}

ko.applyBindings(new RegisterViewModel());