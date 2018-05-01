var pageSize = 10;
var pageIndex = 0;

//$(document).ready(function () {
//    loadMore();
//});

function loadMore() {
    $.ajax({
        type: "POST",
        url: "Home/LoadMore",
        data: { "pageindex": pageIndex, "pagesize": pageSize },
        dataType: "json",
        success: function (response) {
            if (response !== null) {
                var model = new IndexViewModel();
                ko.applyBindings(model);
                pageIndex++;
            }
        },
        failure: function (response) {
            alert("Error while retrieving data!");
        }
    });
}

function likePost(id) {
    $.ajax({
        type: "POST",
        url: "Home/LikePost",
        data: { "id": id },
        dataType: "json",
        success: function (response) {
            alert("Success");
        },
        failure: function (response) {
            alert("Error while retrieving data!");
        }
    });
}

function IndexViewModel() {
    var self = this;
    self.Posts = ko.observableArray([]);

    self.NewPost = ko.observable();
    self.PageSize = pageSize;
    self.PageIndex = pageIndex;
    
    self.GetPostDetails = function() {
        $.ajax({
            type: "POST",
            url: "Home/LoadMore",
            data: { "pageindex": self.PageIndex, "pagesize": self.PageSize },
            dataType: "json",
            success: function (response) {
                console.log(response);
                if (response !== null) {
                    self.Posts($.map(JSON.parse(response), function(post) {
                            return new PostViewModel(post);
                        }));
                }
            },
            failure: function (response) {
                alert("Error while retrieving data!");
            }
        });
    }

    self.CreatePost = function() {
        $.ajax({
            type: "POST",
            url: "Home/CreatePost",
            data: { "newPostContent": self.NewPost(), "pageindex": self.PageIndex, "pagesize": self.PageSize },
            dataType: "json",
            success: function (response) {
                console.log(response);
                if (response !== null) {
                    self.Posts($.map(JSON.parse(response), function (post) {
                        return new PostViewModel(post);
                    }));
                }
            },
            failure: function (response) {
                alert("Error while retrieving data!");
            }
        });
    }
    
    self.GetPostDetails();
}

function PostViewModel(data) {
    var self = this;
    self.Owner = ko.observable(data.owner);
    self.Content = ko.observable(data.content);
    self.PostDate = ko.observable(data.postDate);
    self.LikeCount = ko.observable(data.likeCount);
    self.NewComment = ko.observable();
    self.Comments = ko.observableArray([]);

    self.Comments($.map(data.comments, function (comment) {
        return new CommentViewModel(comment);
    }));

    self.LikePost = function (item, event) {
        var context = ko.contextFor(event.target);
        var index = context.$index();
        $.ajax({
            type: "POST",
            url: "Home/LikePost",
            data: {"id": index},
            dataType: "json",
            success: function (response) {
                console.log("successful ajax response");
                if (response !== null) {
                    self.LikeCount(response);
                }
            },
            failure: function (response) {
                alert("Error while retrieving data!");
            }
        });
    }

    self.CommentPost = function(item, event) {
        var context = ko.contextFor(event.target);
        var index = context.$index();
        $.ajax({
            type: "POST",
            url: "Home/CommentPost",
            data: { "id": index, "comment": self.NewComment()},
            dataType: "json",
            success: function (response) {
                self.Comments($.map(JSON.parse(response),
                    function(comment) {
                        return new CommentViewModel(comment);
                    }));
                self.NewComment("");
            },
            failure: function (response) {
                alert("Error while retrieving data!");
            }
        });
    }
}

function CommentViewModel(data) {
    var self = this;
    self.CommentOwner = ko.observable(data.CommentOwner);
    self.CommentPostDate = ko.observable(data.CommentPostDate);
    self.CommentContent = ko.observable(data.CommentContent);
    self.CommentLikeCount = ko.observable(data.CommentLikeCount);

    self.LikeComment = function (item, event) {
        var context = ko.contextFor(event.target);
        var index = context.$index();
        var indexParent = context.$parentContext.$index();
        $.ajax({
            type: "POST",
            url: "Home/LikeComment",
            data: { "id": index, "idPost": indexParent },
            dataType: "json",
            success: function (response) {
                if (response !== null) {
                    self.CommentLikeCount(response);
                }
            },
            failure: function (response) {
                alert("Error while retrieving data!");
            }
        });
    }
}

ko.applyBindings(new IndexViewModel());