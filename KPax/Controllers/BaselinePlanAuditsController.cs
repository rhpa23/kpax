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
    public class BaselinePlanAuditsController : Controller
    {
        private EntitiesDB db = new EntitiesDB();

        // GET: BaselinePlanAudits
        public ActionResult Index()
        {
            var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
            var baselinePlans = db.BaselinePlan.Where(bp => projects.Contains(bp.Project));

            var baselinePlanAudit = db.BaselinePlanAudit.Where(a => baselinePlans.Contains(a.BaselinePlan));
            return View(baselinePlanAudit.ToList());
        }

        // GET: BaselinePlanAudits/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BaselinePlanAudit baselinePlanAudit = db.BaselinePlanAudit.Find(id);
            if (baselinePlanAudit == null)
            {
                return HttpNotFound();
            }
            return View(baselinePlanAudit);
        }

        // GET: BaselinePlanAudits/Create
        public ActionResult Create()
        {
            var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
            var baselinePlans = db.BaselinePlan.Where(bp => projects.Contains(bp.Project));

            ViewBag.BaselinePlanId = new SelectList(baselinePlans, "Id", "Name");
            var viewModel = new BaselinePlanAuditViewModel();
            viewModel.BaselineAudits = db.BaselineAudit.ToList();
            return View(viewModel);
        }

        // POST: BaselinePlanAudits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BaselinePlanId,Values")] BaselinePlanAuditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var baselinePlanAudit = new BaselinePlanAudit();
                baselinePlanAudit.BaselinePlanId = viewModel.BaselinePlanId;
                baselinePlanAudit.Values = string.Join(",", viewModel.Values);
                baselinePlanAudit.Verified = true;

                db.BaselinePlanAudit.Add(baselinePlanAudit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
            var baselinePlans = db.BaselinePlan.Where(bp => projects.Contains(bp.Project));
            ViewBag.BaselinePlanId = new SelectList(baselinePlans, "Id", "Name", viewModel.BaselinePlanId);
            return View(viewModel);
        }

        // GET: BaselinePlanAudits/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BaselinePlanAudit baselinePlanAudit = db.BaselinePlanAudit.Find(id);
            if (baselinePlanAudit == null)
            {
                return HttpNotFound();
            }

            var viewModel = new BaselinePlanAuditViewModel();
            viewModel.SelectedBaselineAudits = new List<BaselineAudit>();
            viewModel.BaselinePlanId = baselinePlanAudit.BaselinePlanId;
            viewModel.BaselineAudits = db.BaselineAudit.ToList();
            foreach (var auditId in baselinePlanAudit.Values.Trim().Split(','))
            {
                int idAudit;
                if (int.TryParse(auditId, out idAudit))
                    viewModel.SelectedBaselineAudits.Add(db.BaselineAudit.Find(idAudit));
            }

            var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
            var baselinePlans = db.BaselinePlan.Where(bp => projects.Contains(bp.Project));
            ViewBag.BaselinePlanId = new SelectList(baselinePlans, "Id", "Name", baselinePlanAudit.BaselinePlanId);
            return View(viewModel);
        }

        // POST: BaselinePlanAudits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BaselinePlanId,Values")] BaselinePlanAuditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var baselinePlanAudit = db.BaselinePlanAudit.Find(viewModel.Id);
                baselinePlanAudit.Values = string.Join(",", viewModel.Values);
                baselinePlanAudit.BaselinePlanId = viewModel.BaselinePlanId;
                baselinePlanAudit.Verified = true;

                db.Entry(baselinePlanAudit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
            var baselinePlans = db.BaselinePlan.Where(bp => projects.Contains(bp.Project));
            ViewBag.BaselinePlanId = new SelectList(baselinePlans, "Id", "Name", viewModel.BaselinePlanId);
            return View(viewModel);
        }

        // GET: BaselinePlanAudits/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BaselinePlanAudit baselinePlanAudit = db.BaselinePlanAudit.Find(id);
            if (baselinePlanAudit == null)
            {
                return HttpNotFound();
            }
            return View(baselinePlanAudit);
        }

        // POST: BaselinePlanAudits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BaselinePlanAudit baselinePlanAudit = db.BaselinePlanAudit.Find(id);
            db.BaselinePlanAudit.Remove(baselinePlanAudit);
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
