using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KPax.DataModels;
using KPax.Util;
using KPax.Models;
using System.Threading.Tasks;

namespace KPax.Controllers
{
    public class TeamCitySetupController : Controller
    {
        private EntitiesDB db = new EntitiesDB();

        public ActionResult Index()
        {
            int count = db.ProjectUser.Count(i => i.UserName == User.Identity.Name);
            if (count == 0)
            {
                return RedirectToAction("../Projects");
            }

            var listModel = db.TeamCitySetup.ToList();
            var viewList = new List<TeamCityViewModel>();

            var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);

            foreach (var model in listModel)
            {
                var viewModel = new TeamCityViewModel()
                {
                    Id = model.Id,
                    URI = model.Uri.Trim(),
                    Project = projects.FirstOrDefault(m => m.Id == model.IdProject),
                    BuildId = model.BuildId
                };
                viewList.Add(viewModel);
            }
            return View(viewList);
        }

        public ActionResult Create()
        {
            var teamCityViewModel = new TeamCityViewModel();

            var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
            teamCityViewModel.ProjectList = new SelectList(projects.ToList(), "Id", "Name");

            var list = new List<string>();
            list.Add("TaggedForms_Automatedtests");
            list.Add("TaggedForms_BuildAndUnitTests");
            teamCityViewModel.BuildIdsList = new SelectList(list);

            return View(teamCityViewModel);
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "UpdateBuildIdsList")]
        public ActionResult UpdateBuildIdsList(TeamCityViewModel teamCityViewModel)
        {
            PreparToEdit(teamCityViewModel);

            if (teamCityViewModel.Id > 0)
            {
                return View("edit", teamCityViewModel);
            }

            return View("create", teamCityViewModel);    
        }

        private void PreparToEdit(TeamCityViewModel teamCityViewModel)
        {
            var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
            teamCityViewModel.ProjectList = new SelectList(projects, "Id", "Name");
            teamCityViewModel.BuildIdsList = new SelectList(new List<string>());
            try
            {
                var list = BaselineUtil.GetBuildIds(teamCityViewModel);
                teamCityViewModel.BuildIdsList = new SelectList(list);
            }
            catch (Exception ex)
            {
                TempData.Add("UpdateBuildIdsListException", ex.Message);
            }
        }

        // POST: TeamCitySetup/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultipleButton(Name = "action", Argument = "Create")]
        public async Task<ActionResult> Create(TeamCityViewModel viewModel, string returnUrl)
        {
            var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
            viewModel.ProjectList = new SelectList(projects.ToList(), "Id", "Name");
            viewModel.BuildIdsList = new SelectList(new List<string>());

            if (ModelState.IsValid)
            {
                try
                {
                    if (CheckTeamCitySetup(viewModel))
                    {
                        // Save Data 
                        var teamCityModel = new TeamCitySetup()
                        {
                            Uri = viewModel.URI.Trim(),
                            Crendential = new Crypt32().Encrypt(Crypt32.KeyType.UserKey, viewModel.UserName + ";" + viewModel.Password),
                            IdProject = viewModel.Project.Id,
                            BuildId = viewModel.BuildId
                        };
                        db.TeamCitySetup.Add(teamCityModel);

                        db.SaveChanges();

                        TempData["Success"] = "Yes";
                        return RedirectToAction("Index");
                    }
                }
                catch (WebException e)
                {
                    if (e.Response != null)
                    {
                        using (WebResponse response = e.Response)
                        {
                            var httpResponse = (HttpWebResponse) response;
                            if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                            {
                                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "Cannot establish connection with this server.");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Cannot establish connection with this server.");
                    }
                    
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                }

            }

            return View(viewModel);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                db.TeamCitySetup.Remove(db.TeamCitySetup.Find(id));
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }
            return View();
        }

        // GET: TeamCity/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
                var dbTeamCity = db.TeamCitySetup.Find(id);
                if (dbTeamCity != null)
                {
                    var teamCityView = new TeamCityViewModel()
                    {
                        Id = dbTeamCity.Id,
                        URI = dbTeamCity.Uri.Trim(),
                        Project = projects.FirstOrDefault(m => m.Id == dbTeamCity.IdProject),
                        BuildId = dbTeamCity.BuildId,
                        UserName = new Crypt32().Decrypt(dbTeamCity.Crendential).Split(';')[0],
                        Password = new Crypt32().Decrypt(dbTeamCity.Crendential).Split(';')[1]
                    };
                    PreparToEdit(teamCityView);
                    teamCityView.BuildId = dbTeamCity.BuildId;
                    return View(teamCityView);
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }
            return View();
        }

        // POST: TeamCity/Edit/5
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Edit")]
        public ActionResult Edit(int id, TeamCityViewModel viewModel)
        {
            try
            {
                var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
                ViewBag.ProjectsList = new SelectList(projects.ToList(), "Id", "Name");

                if (CheckTeamCitySetup(viewModel))
                {
                    var dbTeamCity = db.TeamCitySetup.Find(id);
                    if (dbTeamCity != null)
                    {
                        dbTeamCity.Uri = viewModel.URI.Trim();
                        dbTeamCity.Crendential =
                            new Crypt32().Encrypt(Crypt32.KeyType.UserKey,
                                viewModel.UserName + ";" + viewModel.Password);
                        dbTeamCity.IdProject = viewModel.Project.Id;
                        dbTeamCity.BuildId = viewModel.BuildId;
                        db.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                PreparToEdit(viewModel);
                ModelState.AddModelError(string.Empty, e.Message);
            }
            return View(viewModel);
        }

        private bool CheckTeamCitySetup(TeamCityViewModel viewModel)
        {
            WebRequest request = WebRequest.Create(viewModel.URI + "/httpAuth/app/rest/latest/");
                    String encoded =
                        System.Convert.ToBase64String(
                            System.Text.Encoding.GetEncoding("ISO-8859-1")
                                .GetBytes(viewModel.UserName + ":" + viewModel.Password));
                    request.Headers.Add("Authorization", "Basic " + encoded);

            using (WebResponse response = request.GetResponse())
            {
                return (((HttpWebResponse) response).StatusCode == HttpStatusCode.OK);
            }
        }
    }
}
