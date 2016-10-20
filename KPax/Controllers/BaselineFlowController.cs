using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using KPax.DataModels;
using KPax.Models;
using KPax.Util;

namespace KPax.Controllers
{
    public class BaselineFlowController : Controller
    {
        private EntitiesDB db = new EntitiesDB();


        /// <summary>
        /// for setup initial view model for all BaselineFlow
        /// </summary>
        private BaselineFlowListViewModel GetBaselineFlowInitialModel()
        {
            //setup properties
            var model = new BaselineFlowListViewModel();
            var selectedBaselineFlow = new List<BaselineFlow>();

            //setup a view model
            model.AvailableBaselineFlow = db.BaselineFlow.ToList();
            model.SelectedBaselineFlow = selectedBaselineFlow;
            return model;
        }

        /// <summary>
        /// for setup view model, after post the user selected BaselineFlow data
        /// </summary>
        private BaselineFlowListViewModel GetBaselineFlowsModel(PostedBaselineFlow postedBaselineFlow)
        {
            // setup properties
            var model = new BaselineFlowListViewModel();
            var selectedBaselineFlow = new List<BaselineFlow>();
            var postedBaselineFlowIds = new string[0];
            if (postedBaselineFlow == null) postedBaselineFlow = new PostedBaselineFlow();

            // if a view model array of posted BaselineFlow ids exists
            // and is not empty,save selected ids
            if (postedBaselineFlow.BaselineFlowIds != null && postedBaselineFlow.BaselineFlowIds.Any())
            {
                postedBaselineFlowIds = postedBaselineFlow.BaselineFlowIds;
            }

            // if there are any selected ids saved, create a list of BaselineFlow
            if (postedBaselineFlowIds.Any())
            {
                selectedBaselineFlow = db.BaselineFlow.ToList()
                 .Where(x => postedBaselineFlowIds.Any(s => x.Id.ToString().Equals(s)))
                 .ToList();
            }

            //setup a view model
            model.AvailableBaselineFlow = db.BaselineFlow.ToList();
            model.SelectedBaselineFlow = selectedBaselineFlow;
            model.PostedBaselineFlows = postedBaselineFlow;
            return model;
        }

        // GET: BaselineFlow
        public ActionResult Index()
        {
            int count = db.ProjectUser.Count(i => i.UserName == User.Identity.Name);
            if (count == 0)
            {
                return RedirectToAction("../Projects");
            }

            var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);

            ViewBag.ProjectsList = new SelectList(projects.ToList(), "Id", "Name");

            ViewBag.BaselineFlowList = new List<BaselineFlowViewModel>();
            ViewBag.BaselineList = new SelectList(new List<BaselinePlan>());

            return View();
        }

        // POST: BaselineFlow/Index
        [HttpPost]
        public ActionResult Index(BaselinePlanViewModel model)
        {
            if (model.Id > 0 && model.Project.Id > 0)
            {
                return Start(model.Project.Id, model.Id);
            }
            ViewBag.ProjectsList = new SelectList(db.Project.ToList(), "Id", "Name");

            var baselineFlowList = new List<BaselineFlow>();

            var projectBaseFlowList = db.BaselineFlowProject.Where(b => b.ProjectId == model.Project.Id);
            foreach (var baselineFlowProject in projectBaseFlowList)
            {
                var baseFlow =
                    db.BaselineFlow.FirstOrDefault(b => b.Id == baselineFlowProject.BaselineFlowId);
                baselineFlowList.Add(baseFlow);
            }

            var baselineList =
                db.BaselinePlan.Where(p => p.IsReleased == false && p.ProjectId == model.Project.Id)
                    .OrderBy(p => p.ReleaseDate);
            ViewBag.BaselineList = new SelectList(baselineList.ToList(), "Id", "Name");
            ViewBag.BaselineFlowList = baselineFlowList;
            
            return View(model);
        }

        public ActionResult Edit(Project selectedProject)
        {
            var baselineFlowInitialModel = GetBaselineFlowInitialModel();
            baselineFlowInitialModel.Project = db.Project.Find(selectedProject.Id);

            var listFlowProject = db.BaselineFlowProject.Where(f => f.ProjectId == selectedProject.Id);

            var listbaselineFlow = new List<BaselineFlow>();
            foreach (var baselineFlowProject in listFlowProject)
            {
                listbaselineFlow.Add(new BaselineFlow() {Id = baselineFlowProject.BaselineFlowId});
                
            }
            baselineFlowInitialModel.SelectedBaselineFlow = listbaselineFlow.AsEnumerable();

            return View(baselineFlowInitialModel);
        }


        [HttpPost]
        public ActionResult Edit(BaselineFlowListViewModel baselineFlowListViewModel)
        {
            var flows = GetBaselineFlowsModel(baselineFlowListViewModel.PostedBaselineFlows);
            
            db.BaselineFlowProject.RemoveRange(
                db.BaselineFlowProject.Where(p => p.ProjectId == baselineFlowListViewModel.Project.Id));
            foreach (var baselineFlow in flows.SelectedBaselineFlow)
            {
                var flowProject = new BaselineFlowProject() { BaselineFlowId = baselineFlow.Id, ProjectId = baselineFlowListViewModel.Project.Id};
                db.BaselineFlowProject.Add(flowProject);
            }
            db.SaveChanges();

            TempData["Success"] = "Saved  with success. ";
            return View(flows);
        }

        public ActionResult Start(int projectId, int baselineId)
        {
            if (baselineId > 0 && projectId > 0)
            {
                var listFlowProject = db.BaselineFlowProject.Where(f => f.ProjectId == projectId);
                var listbaselineFlow = new List<BaselineFlow>();
                var baseFlowProcessViewModel = new BaselineFlowProcessViewModel();
                foreach (var baselineFlowProject in listFlowProject)
                {
                    var flow = db.BaselineFlow.Find(baselineFlowProject.BaselineFlowId);
                    listbaselineFlow.Add(flow);

                    var baselineSelected = db.BaselinePlan.FirstOrDefault(p => p.Id == baselineId);
                    baseFlowProcessViewModel.BaselinePlan = baselineSelected;
                }

                baseFlowProcessViewModel.BaselineFlow = listbaselineFlow.AsEnumerable();
                return View("Start", baseFlowProcessViewModel);
            }
            ViewBag.ProjectsList = new SelectList(db.Project.ToList(), "Id", "Name");
            TempData.Add("Erro", "Select the baseline.");
            return RedirectToAction("../BaselineFlow");
        }


        public ActionResult Process(int baselineFlowId, string baselinePlan, int projectId)
        {
            Thread.Sleep(2000);
            var baselineFlowVm = new BaselineFlowViewModel();
            baselineFlowVm.Status = " ( OK ) ";
            //baselineFlowVm.BaselineFlow = flowDB.BaselineFlow.FirstOrDefault(b => b.Id == baselineFlowId);

            switch (baselineFlowId)
            {
                case 1: // TeamCity Build
                {
                    baselineFlowVm.Status = BaselineUtil.BuildTeamCity(projectId);
                    break;
                }
                case 2: // Git tag
                {
                    baselineFlowVm.Status = BaselineUtil.ApplyTag(projectId, baselinePlan);
                    break;
                }
                case 6: // Send Email
                {
                    baselineFlowVm.Status = BaselineUtil.SendEmail(projectId, baselinePlan, User.Identity.Name);
                    break;
                }
                case 8: // Update baseline plan to released
                {
                    var baselinePlanModel = db.BaselinePlan.FirstOrDefault(p => p.Name == baselinePlan);
                    baselinePlanModel.IsReleased = true;
                    db.SaveChanges();
                    baselineFlowVm.Status = "Ok";
                    break;
                }
                case 9: //Create a audit release                                                                              
                {
                    var baselineAudit = new BaselinePlanAudit();
                    var baselinePlanModel = db.BaselinePlan.FirstOrDefault(p => p.Name == baselinePlan);
                    baselineAudit.BaselinePlanId = baselinePlanModel.Id;
                    baselineAudit.Verified = false;
                    baselineAudit.Values = "";
                    db.BaselinePlanAudit.Add(baselineAudit);
                    db.SaveChanges();
                    break;
                }
            };
            
            
            return PartialView("BaselineFlowControl", baselineFlowVm);
        }

        [HttpPost]
        public ActionResult Start(BaselineFlowProcessViewModel model)
        {
            // TODO: 
            return View();
        }
    }
}