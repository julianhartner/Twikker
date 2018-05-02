function LoginViewModel() {
    var self = this;
    self.Username = ko.observable();
    self.Password = ko.observable();
    self.Message = ko.observable();
    self.Class = ko.observable();

    self.LogIn = function() {
        $.ajax({
            type: "POST",
            url: "LoginUser",
            data: { "username": self.Username(), "password": self.Password() },
            dataType: "json",
            success: function(response) {
                if (response !== null) {
                    var parsedResponse = JSON.parse(response);
                    console.log(parsedResponse);
                    self.Message(parsedResponse.message);

                    if (parsedResponse.type === "success") {
                        self.Class("alert alert-success");
                        location.reload();
                    } else {
                        self.Class("alert alert-danger");
                    }

                }
            },
            failure: function(response) {
                alert("Error while retrieving data!");
            }
        });
    };
}

ko.applyBindings(new LoginViewModel());