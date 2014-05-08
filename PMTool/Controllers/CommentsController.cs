using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMTool.Models;
using PMTool.Repository;
using System.Web.Security;
using System.Threading;

namespace PMTool.Controllers
{   
    public class CommentsController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private PMToolContext context = new PMToolContext(); //It have to remove after code refactoring

        //
        // GET: /Comments/

        public ViewResult Index()
        {
            //return View(unitOfWork.TaskRepository.AllIncluding(task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList());
            //return View(context.Comments.Include(comment => comment.Task).ToList());
            return View(unitOfWork.CommentRepository.AllIncluding(comment => comment.Task).Include(comment => comment.CommentByUser).ToList());
        }

        //
        // GET: /Comments/Details/5

        public ViewResult Details(long id)
        {
            Comment comment = context.Comments.Single(x => x.ID == id);
            return View(comment);
        }

        //
        // GET: /Comments/Create

        public ActionResult Create()
        {
            ViewBag.PossibleTasks = context.Tasks;
            return View();
        } 

        //
        // POST: /Comments/Create

        [HttpPost]
        public ActionResult Create(Comment comment)
        {
            if (ModelState.IsValid)
            {
                context.Comments.Add(comment);
                context.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.PossibleTasks = context.Tasks;
            return View(comment);
        }
        
        //
        // GET: /Comments/Edit/5
 
        public ActionResult Edit(long id)
        {
            Comment comment = context.Comments.Single(x => x.ID == id);
            ViewBag.PossibleTasks = context.Tasks;
            return View(comment);
        }

        //
        // POST: /Comments/Edit/5

        [HttpPost]
        public ActionResult Edit(Comment comment)
        {
            if (ModelState.IsValid)
            {
                context.Entry(comment).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PossibleTasks = context.Tasks;
            return View(comment);
        }

        //
        // GET: /Comments/Delete/5
 
        public ActionResult Delete(long id)
        {
            Comment comment = context.Comments.Single(x => x.ID == id);
            return View(comment);
        }

        //
        // POST: /Comments/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            Comment comment = context.Comments.Single(x => x.ID == id);
            context.Comments.Remove(comment);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        public PartialViewResult _CommentList(long taskId)
        {
            List<CommentViewModel> commentsWithReplyList = new List<CommentViewModel>();
            List<Comment> comments = unitOfWork.CommentRepository.GetComments(taskId);
            foreach (Comment item in comments)
            {

                CommentViewModel commentViewModel = new CommentViewModel();
                commentViewModel.CommentBy = item.CommentBy;
                commentViewModel.CommentByUser = item.CommentByUser;
                commentViewModel.CreateDate = item.CreateDate;
                commentViewModel.ID = item.ID;
                commentViewModel.Message = item.Message;
                commentViewModel.ModifiedDate = item.ModifiedDate;
                commentViewModel.ParentComment = item.ParentComment;
                commentViewModel.Task = item.Task;
                commentViewModel.TaskID = item.TaskID;
                commentViewModel.ReplyComments = unitOfWork.CommentRepository.GetReplyCommentsByID(item.ID);
                commentsWithReplyList.Add(commentViewModel);
                
            }
            return PartialView(commentsWithReplyList);
        }


        public PartialViewResult _PostComments(long id) //id = taskId
        {
            Comment comment = new Comment();
            comment.TaskID = id;
            return PartialView(comment);
        }

        public PartialViewResult _PostReplys()
        {
            return null;
        }

        [HttpPost]
        public ActionResult _PostReplys(Comment comment)
        {
            if (comment != null)
            {
                
                comment.ModifiedDate = DateTime.Now;
                comment.CreateDate = DateTime.Now;
                comment.CommentBy = (int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey;
                //comment.TaskID = 113; //remove it.

                try
                {
                    unitOfWork.CommentRepository.InsertOrUpdate(comment);

                    unitOfWork.Save();


                    Task task = unitOfWork.TaskRepository.Find(comment.TaskID);
                    List<UserProfile> lstInvolvedUser = GetTaskUserOrFollower(comment.TaskID);
                    UserProfile commentby = unitOfWork.UserRepository.GetUserByUserID(comment.CommentBy);
                    string subject = "Comments on task \"" + task.Title + "\"";
                    string body = "<br />" + comment.Message + "";
                    string url = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Content("~") + "/Tasks/Details?id=" + task.TaskID;
                    body += "<br/>Task Title: <a href=\"" + url + "\"target=\"_blank\" shape=\"rect\">" + task.Title + "</a>";
                    body += "<br/>Comment from: " + commentby.FirstName + " " + commentby.LastName;
                    body += "<br /><div><p>We sent you this email because you signed up in PMTool and You are assigned or followed the task. <br /> Please don't reply this mail.</p><p>"
                        + "Regards,<br />PMTool</p></div>";

                    //List of user should be as: to like mahedee@yahoo.com; jamil@gmail.com;
                    //List of follower should be as in cc like mahedee@yahoo.com; jamil@gmail.com;
                    //string emailto = String.Empty;
                    //foreach (var user in lstInvolvedUser)
                    //{
                    //    emailto += user.UserName + ";";
                    //}

                    List<string> emailtolist = new List<string>();
                    foreach (var user in lstInvolvedUser)
                    {
                        emailtolist.Add(user.UserName);
                    }

                    if (emailtolist != null)
                        new Thread(() =>
                        {
                            unitOfWork.EmailProcessor.SendEmail("", emailtolist, null, null, subject, body);
                        }).Start();




                    //unitOfWork.EmailProcessor.SendEmail("",user.UserName,
                }
                catch (Exception exp)
                {

                }
            }
            return RedirectToAction("_CommentList", new { @taskId = comment.TaskID });
            //return RedirectToAction("_TaskEntryLog", new { @taskId = timeLog.TaskID });
        }


        [HttpPost]
        public ActionResult _PostComments(Comment comment)
        {
            if (comment != null)
            {
                comment.ModifiedDate = DateTime.Now;
                comment.CreateDate = DateTime.Now;
                comment.CommentBy = (int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey;
                //comment.TaskID = 113; //remove it.

                try
                {
                    unitOfWork.CommentRepository.InsertOrUpdate(comment);

                    unitOfWork.Save();

                   
                        Task task = unitOfWork.TaskRepository.Find(comment.TaskID);
                        List<UserProfile> lstInvolvedUser = GetTaskUserOrFollower(comment.TaskID);
                        UserProfile commentby = unitOfWork.UserRepository.GetUserByUserID(comment.CommentBy);
                        string subject = "Comments on task \"" + task.Title + "\"";
                        string body = "<br />" + comment.Message + "";
                        string url = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Content("~") + "/Tasks/Details?id=" + task.TaskID;
                        body += "<br/>Task Title: <a href=\"" + url + "\"target=\"_blank\" shape=\"rect\">" + task.Title + "</a>";
                        body += "<br/>Comment from: " + commentby.FirstName + " " + commentby.LastName;
                        body += "<br /><div><p>We sent you this email because you signed up in PMTool and You are assigned or followed the task. <br /> Please don't reply this mail.</p><p>"
                            + "Regards,<br />PMTool</p></div>";

                        //List of user should be as: to like mahedee@yahoo.com; jamil@gmail.com;
                        //List of follower should be as in cc like mahedee@yahoo.com; jamil@gmail.com;
                        //string emailto = String.Empty;
                        //foreach (var user in lstInvolvedUser)
                        //{
                        //    emailto += user.UserName + ";";
                        //}

                        List<string> emailtolist = new List<string>();
                        foreach (var user in lstInvolvedUser)
                        {
                            emailtolist.Add(user.UserName);
                        }

                        if (emailtolist != null)
                            new Thread(() =>
                            {
                            unitOfWork.EmailProcessor.SendEmail("", emailtolist, null, null, subject, body);
                    }).Start();

                    


                    //unitOfWork.EmailProcessor.SendEmail("",user.UserName,
                }
                catch (Exception exp)
                {

                }
            }
            return RedirectToAction("_CommentList", new { @taskId = comment.TaskID });
            //return RedirectToAction("_TaskEntryLog", new { @taskId = timeLog.TaskID });
        }

        private List<UserProfile> GetTaskUserOrFollower(long taskId)
        {
            Task task = unitOfWork.TaskRepository.Find(taskId);
            List<UserProfile> lstofInvolvedUser = task.Users.ToList();
            //List<UserProfile> lstFllowers = task.Followers.ToList();

            lstofInvolvedUser.AddRange(task.Followers.ToList());
            return lstofInvolvedUser;
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}