var pageSize = 10;
var pageIndex = 0;

ko.bindingHandlers.limitCharacters = {
    update: function(element, valueAccessor, allBindingsAccessor) {
        if (allBindingsAccessor().value()) {
            allBindingsAccessor().value(allBindingsAccessor().value().substr(0, valueAccessor()));
        }
    }
};

function IndexViewModel() {
    var self = this;
    self.Posts = ko.observableArray([]);

    self.NewPost = ko.observable();

    self.GetPostDetails = function() {
        $.ajax({
            type: "POST",
            url: "Home/LoadMore",
            data: { "pageindex": pageIndex, "pagesize": pageSize },
            dataType: "json",
            success: function(response) {
                if (response !== null) {
                    console.log(JSON.parse(response));
                    self.Posts($.map(JSON.parse(response),
                        function(post) {
                            return new PostViewModel(post);
                        }));
                    disableButtons();
                }
            },
            failure: function(response) {
                alert("Error while retrieving data!");
            }
        });
    };

    self.LoadMore = function() {
        pageIndex++;
        $.ajax({
            type: "POST",
            url: "Home/LoadMore",
            data: { "pageindex": pageIndex, "pagesize": pageSize },
            dataType: "json",
            success: function(response) {
                if (response !== null && response !== "") {
                    const parsedResponse = JSON.parse(response);
                    console.log(parsedResponse);
                    for (let i = 0; i < parsedResponse.length; i++) {
                        self.Posts.push(new PostViewModel(parsedResponse[i]));
                    }
                    disableButtons();
                } else {
                    pageIndex--;
                }
            },
            failure: function(response) {
                alert("Error while retrieving data!");
            }
        });
    };

    self.CreatePost = function() {
        $.ajax({
            type: "POST",
            url: "Home/CreatePost",
            data: { "newPostContent": self.NewPost(), "pageindex": pageIndex, "pagesize": pageSize },
            dataType: "json",
            success: function(response) {
                if (response !== null) {
                    self.Posts($.map(JSON.parse(response),
                        function(post) {
                            return new PostViewModel(post);
                        }));
                }
            },
            failure: function(response) {
                alert("Error while retrieving data!");
            }
        });
    };

    self.GetPostDetails();
}

function PostViewModel(data) {
    console.log(data);
    var self = this;
    self.Owner = ko.observable(data.owner);
    self.Content = ko.observable(data.content);
    self.PostDate = ko.observable(data.postDate);
    self.LikeCount = ko.observable(data.likeCount);
    self.NewComment = ko.observable("");
    self.Comments = ko.observableArray([]);
    self.TextCounter = ko.computed(function() {
        const newComment = self.NewComment();
        const remaining = 300 - newComment.length;
        return remaining;
    });

    if (data.isLikeable === "True") {
        self.IsPostLikeable = ko.observable("");
    } else {
        self.IsPostLikeable = ko.observable("like-disabled");
    }
        
    if (data.isRemovable === "True")
        self.IsPostRemovable = ko.observable("");
    else
        self.IsPostRemovable = ko.observable("remove-disabled");

    self.LikeCountVisible = ko.computed(function() {
        if (self.LikeCount() > 0) {
            return true;
        } else {
            return false;
        }
    });

    self.Comments($.map(data.comments,
        function(comment) {
            return new CommentViewModel(comment);
        }));

    self.LikePost = function(item, event) {
        const context = ko.contextFor(event.target);
        const index = context.$index();
        $.ajax({
            type: "POST",
            url: "Home/LikePost",
            data: { "id": index },
            dataType: "json",
            success: function(response) {
                console.log("successful ajax response");
                if (response !== null) {
                    self.LikeCount(response);
                }
            },
            failure: function(response) {
                alert("Error while retrieving data!");
            }
        });
    };

    self.CommentPost = function(item, event) {
        const context = ko.contextFor(event.target);
        const index = context.$index();
        $.ajax({
            type: "POST",
            url: "Home/CommentPost",
            data: { "id": index, "comment": self.NewComment() },
            dataType: "json",
            success: function(response) {
                self.Comments($.map(JSON.parse(response),
                    function(comment) {
                        return new CommentViewModel(comment);
                    }));
                self.NewComment("");
            },
            failure: function(response) {
                alert("Error while retrieving data!");
            }
        });
    };

    self.RemovePost = function(item, event) {
        if (!confirm("Are you sure you want to delete this post?")) {
            return;
        }
        const context = ko.contextFor(event.target);
        const index = context.$index();
        $.ajax({
            type: "POST",
            url: "Home/RemovePost",
            data: { "id": index },
            dataType: "json",
            success: function(response) {
                location.reload();
            },
            failure: function(response) {
                alert("Error while retrieving data!");
            }
        });
    };
}

function CommentViewModel(data) {
    var self = this;
    self.CommentOwner = ko.observable(data.CommentOwner);
    self.CommentPostDate = ko.observable(data.CommentPostDate);
    self.CommentContent = ko.observable(data.CommentContent);
    self.CommentLikeCount = ko.observable(data.CommentLikeCount);

    if (data.isLikeable === "True")
        self.IsCommentLikeable = ko.observable("");
    else
        self.IsCommentLikeable = ko.observable("like-disabled");

    if (data.isRemovable === "True")
        self.IsCommentRemovable = ko.observable("");
    else
        self.IsCommentRemovable = ko.observable("remove-disabled");

    self.LikeCountVisible = ko.computed(function() {
        if (self.CommentLikeCount() > 0) {
            return true;
        } else {
            console.log(self.CommentLikeCount() + " => false");
            return false;
        }
    });

    self.LikeComment = function(item, event) {
        const context = ko.contextFor(event.target);
        const index = context.$index();
        const indexParent = context.$parentContext.$index();
        $.ajax({
            type: "POST",
            url: "Home/LikeComment",
            data: { "id": index, "idPost": indexParent },
            dataType: "json",
            success: function(response) {
                if (response !== null) {
                    self.CommentLikeCount(response);
                }
            },
            failure: function(response) {
                alert("Error while retrieving data!");
            }
        });
    };

    self.RemoveComment = function(item, event) {
        const context = ko.contextFor(event.target);
        const index = context.$index();
        const indexParent = context.$parentContext.$index();
        $.ajax({
            type: "POST",
            url: "Home/RemoveComment",
            data: { "id": index, "idPost": indexParent },
            dataType: "json",
            success: function (response) {
                location.reload();
            },
            failure: function (response) {
                alert("Error while retrieving data!");
            }
        });
    };
}


function disableButtons() {
    $("button.like-disabled").attr("disabled", true);
    $("button.remove-disabled").attr("disabled", true);
}


ko.applyBindings(new IndexViewModel());