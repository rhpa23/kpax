using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using KPax.DataModels;

namespace KPax.Controllers
{
    public class ProjectsController : Controller
    {
        private EntitiesDB db = new EntitiesDB();

        // GET: Projects
        public ActionResult Index()
        {
            var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
            var list = projects.ToList();
            if (list.Count == 0)
            {
                ViewBag.Message = "First create a new Project.";
            }
            return View(list);
        }

        // GET: Projects/Details/5
        public ActionResult Details(int id)
        {
            return View(db.Project.Find(id));
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            var model = new Project();
            return View(model);
        }

        // POST: Projects/Create
        [HttpPost]
        public ActionResult Create(Project model, string teamList)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Project.Add(model);
                    db.SaveChanges();

                    SaveProjectUsers(model, teamList);

                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch
            {
                return View();
            }
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int id)
        {
            var proj = db.Project.Find(id);
            var projUsers = db.ProjectUser.Where(pu => pu.ProjectId == proj.Id);
            var teamNames = projUsers.Select(pu => pu.UserName.Trim());
            string teamUsers = string.Join(", ", teamNames);
            ViewBag.teamList = teamUsers.Trim();

            return View(proj);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Project model, string teamList)
        {
            try
            {
                var dbProject = db.Project.Find(id);
                if (dbProject != null)
                {
                    dbProject.Name = model.Name;
                    dbProject.Email = model.Email;
                    dbProject.EmailCM = model.EmailCM;
                    dbProject.Acronym = model.Acronym;
                    dbProject.TemplateEmail = model.TemplateEmail;
                    db.SaveChanges();

                    db.ProjectUser.RemoveRange(db.ProjectUser.Where(pu => pu.ProjectId == id));
                    db.SaveChanges();

                    SaveProjectUsers(model, teamList);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int id)
        {
            db.ProjectUser.RemoveRange(db.ProjectUser.Where(pu => pu.ProjectId == id));
            db.SaveChanges();

            db.Project.Remove(db.Project.Find(id));
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        private void SaveProjectUsers(Project model, string teamList)
        {
            var userList = teamList.Split(',');

            var projUserList = new List<ProjectUser>();
            foreach (var user in userList)
            {
                var projectUserModel = new ProjectUser()
                {
                    UserName = user.Trim(),
                    ProjectId = model.Id
                };
                projUserList.Add(projectUserModel);
            }

            if (!projUserList.Exists(
                    u => String.Equals(u.UserName, User.Identity.Name, StringComparison.CurrentCultureIgnoreCase)))
            {
                var projectUserModel = new ProjectUser()
                {
                    UserName = User.Identity.Name,
                    ProjectId = model.Id
                };
                projUserList.Add(projectUserModel);
            }

            db.ProjectUser.AddRange(projUserList);
            db.SaveChanges();
        }
    }
}
