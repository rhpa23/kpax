using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KPax.DataModels;
using KPax.Models;
using KPax.Util;

namespace KPax.Controllers
{
    public class GitSetupController : Controller
    {
        private EntitiesDB db = new EntitiesDB();

        // GET: GitSetup
        public ActionResult Index()
        {
            int count = db.ProjectUser.Count(i => i.UserName == User.Identity.Name);
            if (count == 0)
            {
                return RedirectToAction("../Projects");
            }

            var listModel = db.GitSetup.ToList();
            var viewList = new List<GitSetupViewModel>();
            var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
            foreach (var model in listModel)
            {
                var viewModel = new GitSetupViewModel()
                {
                    Id = model.Id,
                    RepositoryPath = model.RepositoryPath,
                    Project = projects.FirstOrDefault(m => m.Id == model.ProjectId),
                    Description = model.Description
                };
                viewList.Add(viewModel);
            }
            //new GitSharpClient().OpenRepo();
            return View(viewList);
        }

        // GET: GitSetup/Details/5
        public ActionResult Details(int id)
        {
            var model = db.GitSetup.FirstOrDefault(i => i.Id == id);
            var gitSharpClient =new GitSharpClient(model.RepositoryPath);

            ViewBag.CurrentBranch = gitSharpClient.CurrentBranch().FriendlyName;
            ViewBag.LastTag = gitSharpClient.LastTag();
            ViewBag.LastModify = gitSharpClient.LastModify();
            ViewBag.Status = gitSharpClient.Status();
            
            return Edit(id);
        }

        public ActionResult Pull(int id)
        {
            try
            {
                var model = db.GitSetup.FirstOrDefault(i => i.Id == id);
                string credential = new Crypt32().Decrypt(model.Crendential);
                var status = new GitSharpClient(model.RepositoryPath).Pull(credential.Split('|')[0], credential.Split('|')[1]);
                TempData["Success"] = "Git pull command process with success. " + status;
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
            }

            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        // GET: GitSetup/Create
        public ActionResult Create()
        {
            var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
            ViewBag.ProjectsList = new SelectList(projects.ToList(), "Id", "Name");
            return View();
        }

        // POST: GitSetup/Create
        [HttpPost]
        public ActionResult Create(GitSetupViewModel viewModel, string returnUrl)
        {
            var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
            ViewBag.ProjectsList = new SelectList(projects.ToList(), "Id", "Name");

            if (ModelState.IsValid)
            {
                try
                {
                    //if (CheckGitSetup(viewModel))
                    //{
                        // Save Data 
                        var gitModel = new GitSetup()
                        {
                            RepositoryPath = viewModel.RepositoryPath.Trim(),
                            ProjectId = viewModel.Project.Id,
                            Crendential = new Crypt32().Encrypt(Crypt32.KeyType.UserKey, viewModel.UserName + "|" + viewModel.Password),
                            Description = viewModel.Description
                        };
                        db.GitSetup.Add(gitModel);

                        db.SaveChanges();

                        return RedirectToAction("Index");
                    //}
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                }

            }

            return View();
        }

        // GET: GitSetup/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
                ViewBag.ProjectsList = new SelectList(projects.ToList(), "Id", "Name");

                var gitModel = db.GitSetup.Find(id);
                if (gitModel != null)
                {

                    var gitView = new GitSetupViewModel()
                    {
                        Id = gitModel.Id,
                        RepositoryPath = gitModel.RepositoryPath.Trim(),
                        Project = db.Project.FirstOrDefault(m => m.Id == gitModel.ProjectId),
                        UserName = new Crypt32().Decrypt(gitModel.Crendential).Split('|')[0],
                        Description = gitModel.Description
                    };
                    return View(gitView);
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }
            return View();
        }

        // POST: GitSetup/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, GitSetupViewModel viewModel)
        {
            try
            {
                var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
                ViewBag.ProjectsList = new SelectList(projects.ToList(), "Id", "Name");

                //if (CheckGitSetup(viewModel))
                //{
                    var gitModel = db.GitSetup.Find(id);
                    if (gitModel != null)
                    {
                        gitModel.RepositoryPath = viewModel.RepositoryPath.Trim();
                        gitModel.ProjectId = viewModel.Project.Id;
                        gitModel.Crendential =
                            new Crypt32().Encrypt(Crypt32.KeyType.UserKey,
                                viewModel.UserName + "|" + viewModel.Password);
                        gitModel.Description = viewModel.Description;
                        db.SaveChanges();
                    }

                    return RedirectToAction("Index");
                //}
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }
            return View();
        }

        // GET: GitSetup/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                db.GitSetup.Remove(db.GitSetup.Find(id));
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }
            return View();
        }
    }
}
