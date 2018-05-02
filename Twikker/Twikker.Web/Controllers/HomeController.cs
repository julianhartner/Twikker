using System;
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

            var json = "[";


            foreach (var twikkerPost in posts)
            {
                json +=
                    $"{{\"owner\":\"{twikkerPost.Owner}\", \"content\":\"{twikkerPost.Content}\", \"postDate\":\"{twikkerPost.PostDate}\", \"likeCount\":\"{twikkerPost.Likes.Count}\", \"id\":\"{twikkerPost.ID}\"";

                if (twikkerPost.Comments.Count > 0)
                {
                    json += ",\"comments\":[";

                    foreach (var comment in twikkerPost.Comments)
                        json +=
                            $"{{\"CommentOwner\":\"{comment.Owner.UserName}\", \"CommentContent\":\"{comment.Content}\", \"CommentPostDate\":\"{comment.PostDate}\", \"CommentLikeCount\":\"{comment.Likes.Count}\"}},";

                    json = json.Substring(0, json.Length - 1);
                    json += "]";
                }

                json += "},";
            }

            json = json.Substring(0, json.Length - 1);

            if (json.Length > 0)
                json += "]";

            return Json(json);
        }

        public ActionResult LoadMore(int pageIndex, int pageSize)
        {
            var tempPosts = _context.Posts
                .Include(p => p.Comments)
                .ThenInclude(c => c.Owner)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Likes)
                .Include(p => p.Owner)
                .Include(p => p.Likes)
                //.Skip(pageIndex * pageSize)
                //.Take(pageSize)
                .AsNoTracking().ToList();

            tempPosts.Sort();
            var posts = tempPosts.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            var json = "[";


            foreach (var twikkerPost in posts)
            {
                json +=
                    $"{{\"owner\":\"{twikkerPost.Owner}\", \"content\":\"{twikkerPost.Content}\", \"postDate\":\"{twikkerPost.PostDate}\", \"likeCount\":\"{twikkerPost.Likes.Count}\", \"id\":\"{twikkerPost.ID}\"";

                if (twikkerPost.Comments.Count > 0)
                {
                    json += ",\"comments\":[";

                    foreach (var comment in twikkerPost.Comments)
                        json +=
                            $"{{\"CommentOwner\":\"{comment.Owner.UserName}\", \"CommentContent\":\"{comment.Content}\", \"CommentPostDate\":\"{comment.PostDate}\", \"CommentLikeCount\":\"{comment.Likes.Count}\"}},";

                    json = json.Substring(0, json.Length - 1);
                    json += "]";
                }

                json += "},";
            }

            json = json.Substring(0, json.Length - 1);

            if (json.Length > 0)
                json += "]";

            return Json(json);
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

            var json = "[";

            foreach (var item in post.Comments)
                json +=
                    $"{{\"CommentOwner\":\"{item.Owner.UserName}\", \"CommentContent\":\"{item.Content}\", \"CommentPostDate\":\"{item.PostDate}\", \"CommentLikeCount\":\"{item.Likes.Count}\"}},";

            json = json.Substring(0, json.Length - 1);
            json += "]";

            return Json(json);
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

        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}