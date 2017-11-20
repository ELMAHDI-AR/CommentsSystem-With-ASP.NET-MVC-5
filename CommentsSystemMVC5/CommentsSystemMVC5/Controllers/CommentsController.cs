using CommentsSystemMVC5.EDMX;
using CommentsSystemMVC5.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommentsSystemMVC5.Controllers
{
    public class CommentsController : Controller
    {
        //DbContext
        public DBCommentsEntities dbContext = new DBCommentsEntities();

        // GET: Comments
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetUsers()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetUsers(string username)
        {
            User user = dbContext.Users.Where(u => u.Username.ToLower() == username.ToLower())
                                 .FirstOrDefault();

            if(user != null)
            {
                Session["UserID"] = user.UserID;
                return RedirectToAction("GetPosts");
            }

            ViewBag.Msg = "Username does not exist !";
            return View();
        }

        [HttpGet]
        public ActionResult GetPosts()
        {
            IQueryable<PostsVM> Posts = dbContext.Posts
                                                 .Select(p => new PostsVM
                                                 {
                                                     PostID = p.PostID,
                                                     Message = p.Message,
                                                     PostedDate = p.PostedDate.Value
                                                 }).AsQueryable();

            return View(Posts);
        }

        public PartialViewResult GetComments(int postId)
        {
            IQueryable<CommentsVM> comments = dbContext.Comments.Where(c => c.Post.PostID == postId)
                                                       .Select(c => new CommentsVM
                                                       {
                                                           ComID = c.ComID,
                                                           CommentedDate = c.CommentedDate.Value,
                                                           CommentMsg = c.CommentMsg,
                                                           Users = new UserVM
                                                           {
                                                               UserID = c.User.UserID,
                                                               Username = c.User.Username,
                                                               imageProfile = c.User.imageProfile
                                                           }
                                                       }).AsQueryable();

            return PartialView("~/Views/Shared/_MyComments.cshtml", comments);
        }

        [HttpPost]
        public ActionResult AddComment(CommentsVM comment, int postId)
        {
            //bool result = false;
            Comment commentEntity = null;
            int userId = (int)Session["UserID"];

            var user = dbContext.Users.FirstOrDefault(u => u.UserID == userId);
            var post = dbContext.Posts.FirstOrDefault(p => p.PostID == postId);

            if (comment != null)
            {
                
                commentEntity = new EDMX.Comment
                {
                    CommentMsg = comment.CommentMsg,
                    CommentedDate = comment.CommentedDate,
                };
                

                if (user != null && post != null)
                {
                    post.Comments.Add(commentEntity);
                    user.Comments.Add(commentEntity);

                    dbContext.SaveChanges();
                    //result = true;
                }
            }

            return RedirectToAction("GetComments", "Comments", new { postId  = postId });
        }

        [HttpGet]
        public PartialViewResult GetSubComments(int ComID)
        {
            IQueryable<SubCommentsVM> subComments = dbContext.SubComments.Where(sc => sc.Comment.ComID == ComID)
                                                              .Select(sc => new SubCommentsVM
                                                               {
                                                                  SubComID = sc.SubComID,
                                                                  CommentMsg = sc.CommentMsg,
                                                                  CommentedDate = sc.CommentedDate.Value,
                                                                  User = new UserVM {
                                                                      UserID = sc.User.UserID,
                                                                      Username = sc.User.Username,
                                                                      imageProfile = sc.User.imageProfile
                                                                  }
                                                               }).AsQueryable();

            return PartialView("~/Views/Shared/_MySubComments.cshtml", subComments);
        }

        [HttpPost]
        public ActionResult AddSubComment(SubCommentsVM subComment, int ComID)
        {
            SubComment subCommentEntity = null;
            int userId = (int)Session["UserID"];

            var user = dbContext.Users.FirstOrDefault(u => u.UserID == userId);
            var comment = dbContext.Comments.FirstOrDefault(p => p.ComID == ComID);

            if (subComment != null)
            {

                subCommentEntity = new EDMX.SubComment
                {
                    CommentMsg = subComment.CommentMsg,
                    CommentedDate = subComment.CommentedDate,
                };


                if (user != null && comment != null)
                {
                    comment.SubComments.Add(subCommentEntity);
                    user.SubComments.Add(subCommentEntity);

                    dbContext.SaveChanges();
                    //result = true;
                }
            }

            return RedirectToAction("GetSubComments", "Comments", new { ComID = ComID });

        }
    }
}