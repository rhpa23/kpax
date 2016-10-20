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
    public class AuditsController : Controller
    {
        private EntitiesDB db = new EntitiesDB();

        // GET: Audits
        public ActionResult Index()
        {
            var audits = db.Audits.Include(a => a.BaselinePlan).Include(a => a.Project);
            return View(audits.ToList());
        }

        // GET: Audits/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Audits audits = db.Audits.Find(id);
            if (audits == null)
            {
                return HttpNotFound();
            }
            return View(audits);
        }

        // GET: Audits/Create
        public ActionResult Create()
        {
            var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);

            ViewBag.BaselineId = new SelectList(new List<BaselinePlan>{}, "Id", "Name");
            ViewBag.ProjectId = new SelectList(projects, "Id", "Name");
            return View();
        }

        public JsonResult UpdateBaselines(string selection)
        {
            var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
            ViewBag.ProjectId = new SelectList(projects, "Id", "Name");
            int projectId = int.Parse(selection);
            var baselineData = new SelectList(db.BaselinePlan.Where(b => b.ProjectId == projectId), "Id", "Name");

            return Json(baselineData, JsonRequestBehavior.AllowGet);
        }

        // POST: Audits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProjectId,ReportNumber,ScheduledDate,PerformedDate,Auditor,Comments,BaselineId")] AuditsViewModel auditViewModel)
        {
            if (ModelState.IsValid)
            {
                var audit = new Audits()
                {
                    Auditor = auditViewModel.Auditor,
                    BaselineId = auditViewModel.BaselineId,
                    Project = auditViewModel.Project,
                    PerformedDate = auditViewModel.PerformedDate,
                    Comments = auditViewModel.Comments,
                    BaselinePlan = auditViewModel.BaselinePlan,
                    ProjectId = auditViewModel.ProjectId,
                    ReportNumber = auditViewModel.ReportNumber,
                    ScheduledDate = auditViewModel.ScheduledDate
                };

                db.Audits.Add(audit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
            ViewBag.BaselineId = new SelectList(db.BaselinePlan.Where(b => b.ProjectId == auditViewModel.ProjectId), "Id", "Name");
            ViewBag.ProjectId = new SelectList(projects, "Id", "Name", auditViewModel.ProjectId);
            return View(auditViewModel);
        }

        // GET: Audits/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Audits audits = db.Audits.Find(id);
            if (audits == null)
            {
                return HttpNotFound();
            }
            else
            {
                var auditsViewModel = new AuditsViewModel()
                {
                    Auditor = audits.Auditor,
                    Project = audits.Project,
                    BaselineId = audits.BaselineId,
                    BaselinePlan = audits.BaselinePlan,
                    Comments = audits.Comments,
                    Id = audits.Id,
                    PerformedDate = audits.PerformedDate,
                    ProjectId = audits.ProjectId,
                    ReportNumber = audits.ReportNumber,
                    ScheduledDate = audits.ScheduledDate
                };
                var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
                ViewBag.ProjectsList = new SelectList(projects, "Id", "Name");
                ViewBag.BaselineList = new SelectList(db.BaselinePlan.Where(b => b.ProjectId == audits.ProjectId), "Id", "Name");
                return View(auditsViewModel);
            }
        }

        // POST: Audits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProjectId,ReportNumber,ScheduledDate,PerformedDate,Auditor,Comments,BaselineId")] Audits audits)
        {
            if (ModelState.IsValid)
            {
                db.Entry(audits).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BaselineId = new SelectList(db.BaselinePlan, "Id", "Name", audits.BaselineId);
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name", audits.ProjectId);
            return View(audits);
        }

        // GET: Audits/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Audits audits = db.Audits.Find(id);
            if (audits == null)
            {
                return HttpNotFound();
            }
            return View(audits);
        }

        // POST: Audits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Audits audits = db.Audits.Find(id);
            db.Audits.Remove(audits);
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


        public ActionResult Report(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Audits audit = db.Audits.Find(id);
            if (audit != null)
            {
                var auditReportViewModel = new AuditReportViewModel();
                auditReportViewModel.Audit = audit;
                auditReportViewModel.AuditReports = db.AuditReport.ToList();
                auditReportViewModel.Id = id;

                auditReportViewModel.MyResults = db.AuditReportResults.Where(r => r.AuditId == id).ToList();

                if (audit.PerformedDate == null || auditReportViewModel.MyResults.Count == 0)
                {
                    for (int i = 0; i < auditReportViewModel.AuditReports.Count; i++)
                    {
                        auditReportViewModel.MyResults.Add(new AuditReportResults()
                        {
                            AuditId = id,
                            AuditReportId = auditReportViewModel.AuditReports[i].Id
                        });
                    }
                }
                return View(auditReportViewModel);
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        [HttpPost]
        public ActionResult Report(AuditReportViewModel viewModel)
        {
            Audits audit = db.Audits.Find(viewModel.Id);
            audit.PerformedDate = DateTime.Now;
            db.Entry(audit).State = EntityState.Modified;

            var listResults = db.AuditReportResults.Where(r => r.AuditId == viewModel.Id);
            db.AuditReportResults.RemoveRange(listResults); // Remove old values
            db.SaveChanges();

            db.AuditReportResults.AddRange(viewModel.MyResults); // Insert new values
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
