using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KPax.DataModels;
using KPax.Models;

namespace KPax.Controllers
{
    public class BaselinePlanController : Controller
    {
        private EntitiesDB db = new EntitiesDB();

        // GET: BaselinePlan
        public ActionResult Index()
        {
            var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);

            var listModel = db.BaselinePlan.ToList();
            var viewList = new List<BaselinePlanViewModel>();
            foreach (var model in listModel)
            {
                var viewModel = new BaselinePlanViewModel()
                {
                    Id = model.Id,
                    Name = model.Name,
                    Project = projects.FirstOrDefault(m => m.Id == model.ProjectId),
                    Description = model.Description,
                    IsReleased = model.IsReleased,
                    ReleaseDate = model.ReleaseDate,
                    RememberMe = model.RememberMe,
                    RememberMeDays = model.RememberMeDays
                };
                viewList.Add(viewModel);
            }
            return View(viewList);
        }

        // GET: BaselinePlan/Create
        public ActionResult Create()
        {
            var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
            ViewBag.ProjectsList = new SelectList(projects.ToList(), "Id", "Name");
            return View();
        }

        // POST: BaselinePlan/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BaselinePlanViewModel viewModel)
        {
            var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
            ViewBag.ProjectsList = new SelectList(projects.ToList(), "Id", "Name");

            if (ModelState.IsValid)
            {
                try
                {
                    // Save Data 
                    var baselinePlan = new BaselinePlan()
                    {
                        Name =  viewModel.Name,
                        Description =  viewModel.Description,
                        ProjectId = viewModel.Project.Id,
                        IsReleased = viewModel.IsReleased,
                        ReleaseDate = viewModel.ReleaseDate,
                        RememberMe = viewModel.RememberMe,
                        RememberMeDays = viewModel.RememberMeDays
                    };
                    db.BaselinePlan.Add(baselinePlan);

                    db.SaveChanges();

                    TempData["Success"] = "Yes";
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                }
            }

            return View();
        }

        // GET: BaselinePlan/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
                ViewBag.ProjectsList = new SelectList(projects.ToList(), "Id", "Name");

                var baselinePlan = db.BaselinePlan.Find(id);
                if (baselinePlan != null)
                {
                    var viewModel = new BaselinePlanViewModel()
                    {
                        Name = baselinePlan.Name,
                        Description = baselinePlan.Description,
                        Project = db.Project.FirstOrDefault(m => m.Id == baselinePlan.ProjectId),
                        IsReleased = baselinePlan.IsReleased,
                        ReleaseDate = baselinePlan.ReleaseDate,
                        RememberMe = baselinePlan.RememberMe,
                        RememberMeDays = baselinePlan.RememberMeDays
                    };
                    return View(viewModel);
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }
            return View();
        }

        // POST: BaselinePlan/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, BaselinePlanViewModel viewModel)
        {
            try
            {
                var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
                ViewBag.ProjectsList = new SelectList(projects.ToList(), "Id", "Name");

                var baselinePlan = db.BaselinePlan.Find(id);
                if (baselinePlan != null)
                {
                    baselinePlan.Name = viewModel.Name;
                    baselinePlan.Description = viewModel.Description;
                    baselinePlan.ProjectId = viewModel.Project.Id;
                    baselinePlan.IsReleased = viewModel.IsReleased;
                    baselinePlan.ReleaseDate = viewModel.ReleaseDate;
                    baselinePlan.RememberMe = viewModel.RememberMe;
                    baselinePlan.RememberMeDays = viewModel.RememberMeDays;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }
            return View();
        }

        // GET: BaselinePlan/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                db.BaselinePlan.Remove(db.BaselinePlan.Find(id));
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }
            return View();
        }

        // POST: BaselinePlan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var baselinePlan = db.BaselinePlan.Find(id);
            db.BaselinePlan.Remove(baselinePlan);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
