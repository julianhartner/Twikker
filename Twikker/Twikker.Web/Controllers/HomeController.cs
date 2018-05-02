using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Twikker.Data;
using Twikker.Data.Models;
using Twikker.Web.Models;

namespace Twikker.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly TwikkerDataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(UserManager<ApplicationUser> userManager, TwikkerDataContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> CreatePost(string newPostContent, int pageIndex, int pageSize)
        {
            var newPost = new TwikkerPost
            {
                Owner = await GetCurrentUserAsync(),
                Content = newPostContent,
                PostDate = DateTime.Now
            };

            _context.Add(newPost);
            await _context.SaveChangesAsync();

            var tempPosts = await _context.Posts
                .Include(p => p.Comments)
                .ThenInclude(c => c.Owner)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Likes)
                .Include(p => p.Owner)
                .Include(p => p.Likes)
                .AsNoTracking().ToListAsync();

            tempPosts.Sort();
            var posts = tempPosts.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            return Json(await BuildJsonStringForPosts(posts));
        }

        public async Task<IActionResult> LoadMore(int pageIndex, int pageSize)
        {
            var tempPosts = await _context.Posts
                .Include(p => p.Comments)
                .ThenInclude(c => c.Owner)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Likes)
                .Include(p => p.Owner)
                .Include(p => p.Likes)
                .ToListAsync();

            tempPosts.Sort();
            var posts = tempPosts.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            return Json(await BuildJsonStringForPosts(posts));
        }

        public async Task<ActionResult> LikePost(int id)
        {
            var posts = await _context.Posts
                .Include(p => p.Likes)
                .AsNoTracking().ToListAsync();

            posts.Sort();
            var post = posts[id];

            post.Likes.Add(new PostLike {Post = post, User = await GetCurrentUserAsync()});
            _context.Update(post);
            await _context.SaveChangesAsync();

            return Json(post.Likes.Count);
        }

        public async Task<ActionResult> CommentPost(int id, string comment)
        {
            if (comment.Length > 300)
                comment = comment.Substring(0, 300);

            var posts = await _context.Posts
                .Include(p => p.Comments)
                .ThenInclude(c => c.Likes)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Owner)
                .ToListAsync();

            posts.Sort();
            var post = posts[id];

            post.Comments.Add(new TwikkerComment {Content = comment, Owner = await GetCurrentUserAsync(), PostDate = DateTime.Now});
            _context.Update(post);
            await _context.SaveChangesAsync();

            return Json(await BuildJsonStringForComments(post.Comments));
        }

        public async Task<ActionResult> LikeComment(int id, int idPost)
        {
            var posts = await _context.Posts
                .Include(p => p.Comments)
                .ThenInclude(c => c.Likes)
                .AsNoTracking().ToListAsync();

            posts.Sort();
            var post = posts[idPost];

            var comment = post.Comments.ToList()[id];

            comment.Likes.Add(new CommentLike {Comment = comment, User = await GetCurrentUserAsync()});
            _context.Update(comment);
            await _context.SaveChangesAsync();

            return Json(comment.Likes.Count);
        }

        public async Task<ActionResult> GetComments(int id)
        {
            var posts = await _context.Posts
                .Include(p => p.Owner)
                .Include(p => p.Likes)
                .AsNoTracking().ToListAsync();

            posts.Sort();
            var post = posts[id];
            var comments = post.Comments;

            return Json(comments);
        }

        public async Task<IActionResult> RemovePost(int id)
        {
            var posts = await _context.Posts
                .AsNoTracking()
                .ToListAsync();

            posts.Sort();
            var post = posts[id];

            _context.Remove(post);
            await _context.SaveChangesAsync();

            return Json("");
        }

        public async Task<IActionResult> RemoveComment(int id, int idPost)
        {
            var posts = await _context.Posts
                .Include(p => p.Comments)
                .AsNoTracking().ToListAsync();

            posts.Sort();
            var post = posts[idPost];

            var comment = post.Comments.ToList()[id];

            _context.Remove(comment);
            await _context.SaveChangesAsync();

            return Json("");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private async Task<string> BuildJsonStringForPosts(IEnumerable<TwikkerPost> posts)
        {
            var currentUser = await GetCurrentUserAsync();

            var json = "[";

            foreach (var twikkerPost in posts)
            {
                var isPostRemovable = twikkerPost.Owner == currentUser;
                var isPostLikeable = twikkerPost.Likes.All(p => p.User != currentUser) && User.Identity.IsAuthenticated;

                json +=
                    $"{{\"owner\":\"{twikkerPost.Owner}\", \"content\":\"{twikkerPost.Content}\", \"postDate\":\"{twikkerPost.PostDate}\", \"likeCount\":\"{twikkerPost.Likes.Count}\", \"id\":\"{twikkerPost.ID}\", \"isRemovable\": \"{isPostRemovable}\", \"isLikeable\": \"{isPostLikeable}\"";

                if (twikkerPost.Comments.Count > 0)
                {
                    json += ",\"comments\":";
                    json += await BuildJsonStringForComments(twikkerPost.Comments);
                }

                json += "},";
            }

            json = json.Substring(0, json.Length - 1);

            if (json.Length > 0)
                json += "]";

            return json;
        }

        private async Task<string> BuildJsonStringForComments(IEnumerable<TwikkerComment> comments)
        {
            var currentUser = await GetCurrentUserAsync();

            var json = "[";

            foreach (var comment in comments)
            {
                var isCommentRemovable = comment.Owner == currentUser;
                var isCommentLikeable = comment.Likes.All(p => p.User != currentUser) && User.Identity.IsAuthenticated;

                json +=
                    $"{{\"CommentOwner\":\"{comment.Owner.UserName}\", \"CommentContent\":\"{comment.Content}\", \"CommentPostDate\":\"{comment.PostDate}\", \"CommentLikeCount\":\"{comment.Likes.Count}\", \"isRemovable\": \"{isCommentRemovable}\", \"isLikeable\": \"{isCommentLikeable}\"}},";
            }

            json = json.Substring(0, json.Length - 1);
            json += "]";

            return json;
        }
    }
}