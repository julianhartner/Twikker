function ManageViewModel() {
    var self = this;
    self.OldPassword = ko.observable("");
    self.Password = ko.observable("");
    self.ConfirmPassword = ko.observable("");

    self.ChangePassword = function () {
            $.ajax({
                type: "POST",
                url: "ChangePassword",
                data: { "oldPassword": self.OldPassword(), "password": self.Password(), "confirmPassword": self.ConfirmPassword() },
            dataType: "json",
            success: function (response) {
                if (response !== null) {
                    console.log(response);
                }
            },
            failure: function (response) {
                alert("Error while retrieving data!");
            }
        });
    }
}

ko.applyBindings(new ManageViewModel());