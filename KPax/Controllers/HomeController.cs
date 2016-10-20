using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using KPax.DataModels;
using KPax.Models;
using KPax.Util;

namespace KPax.Controllers
{
    public class HomeController : Controller
    {
        private EntitiesDB db = new EntitiesDB();

        private string SELECTED_PROJECT_KEY = "selected_project";

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Logout()
        {
            ViewBag.Message = "You logout.";

            HttpCookie cookie = Request.Cookies["TSWA-Last-User"];

            if (User.Identity.IsAuthenticated == false || cookie == null || StringComparer.OrdinalIgnoreCase.Equals(User.Identity.Name, cookie.Value))
            {
                string name = string.Empty;

                if (Request.IsAuthenticated)
                {
                    name = User.Identity.Name;
                }

                cookie = new HttpCookie("TSWA-Last-User", name);
                Response.Cookies.Set(cookie);

                Response.AppendHeader("Connection", "close");
                Response.StatusCode = 401; // Unauthorized;
                Response.Clear();
                //should probably do a redirect here to the unauthorized/failed login page
                //if you know how to do this, please tap it on the comments below
                Response.Write("Unauthorized. Reload the page to try again...");
                Response.End();

                return RedirectToAction("Index");
            }

            cookie = new HttpCookie("TSWA-Last-User", string.Empty)
            {
                Expires = DateTime.Now.AddYears(-5)
            };

            Response.Cookies.Set(cookie);

            return RedirectToAction("Index");
        }

        public ActionResult Index()
        {
            var projects = db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);

            ViewBag.ProjectsList = new SelectList(projects.ToList(), "Id", "Name");

            int projectId;
            if (int.TryParse(Request.Cookies[SELECTED_PROJECT_KEY].Value, out projectId))
            {
                
                var project = db.Project.FirstOrDefault(p => p.Id == projectId);
                var homeViewModel = new HomeViewModel()
                {
                    Project = project
                };
                UpdateDashboard(homeViewModel);
                return View(homeViewModel);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(HomeViewModel homeViewModel)
        {
            try
            {
                var projects =
                        db.ProjectUser.Where(pu => pu.UserName.Trim() == User.Identity.Name).Select(pu => pu.Project);
                ViewBag.ProjectsList = new SelectList(projects.ToList(), "Id", "Name");

                UpdateDashboard(homeViewModel);

                var httpCookie = new HttpCookie(SELECTED_PROJECT_KEY);
                httpCookie.Value = homeViewModel.Project.Id.ToString();
                httpCookie.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Set(httpCookie);
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
            }

            return View();
        }

        private void UpdateDashboard(HomeViewModel homeViewModel)
        {
            var htmlPanel = "<div class='row'>";

            InsertGitPanel(ref htmlPanel, homeViewModel.Project.Id);
            InsertTeamCityPanel(ref htmlPanel, homeViewModel.Project.Id);
            InsertBaselinePanel(ref htmlPanel, homeViewModel.Project.Id);
            InsertAuditsPanel(ref htmlPanel, homeViewModel.Project.Id);

            htmlPanel += "</div>";

            ViewBag.Dashboard = htmlPanel;
        }

        private void InsertGitPanel(ref string htmlPanel, int projectId)
        {
            var gitList = db.GitSetup.Where(g => g.ProjectId == projectId);
            foreach (var gitSetup in gitList)
            {
                var gitClient = new GitSharpClient(gitSetup.RepositoryPath);
                var template = string.Format(PanelDashboardTemplate(), "<img src='../../Images/logoGIT.png' width=45px height:35px></img>",
                    "<h5 style='word-wrap: break-word'><b>To clone:</b> " + gitSetup.Description + "</h5>" +
                    "<h5 style='word-wrap: break-word'><b>Last modify:</b> " + gitClient.LastModify() + "</h5>");
                htmlPanel += template;
            }
        }

        private void InsertTeamCityPanel(ref string htmlPanel, int projectId)
        {
            var teamCityList = db.TeamCitySetup.Where(t => t.IdProject == projectId);
            foreach (var teamCitySetup in teamCityList)
            {
                var lastBuildId = BaselineUtil.GetTeamCityLastBuildId(teamCitySetup);
                var tcBuildTO = BaselineUtil.GetBuildStatus(lastBuildId, teamCitySetup);
                var status = tcBuildTO.Status + " Build: " + tcBuildTO.Number + " Started at: " + tcBuildTO.StartDate + " - " + tcBuildTO.StatusText;
                var template = string.Format(PanelDashboardTemplate(), "<img src='../../Images/logoTC.png' width=27px height:25px></img> TeamCity",
                    "<h5 style='word-wrap: break-word'><b>Last build:</b> " + status + "</h5>" +
                    "<h5 style='word-wrap: break-word'><b>Link:</b> <a href='" + teamCitySetup.Uri + "' target='_blank'>" + teamCitySetup.Uri + "</a>  </h5>");
                htmlPanel += template;
            }
        }

        private void InsertBaselinePanel(ref string htmlPanel, int projectId)
        {
            var baslineList = db.BaselinePlan.Where(t => t.ProjectId == projectId);
            if (baslineList.Any())
            {
                var pendingAudit =
                    db.BaselinePlanAudit.FirstOrDefault(a => !a.Verified && a.BaselinePlan.ProjectId == projectId);
                var last = baslineList.OrderBy(b => b.ReleaseDate).FirstOrDefault(s => s.IsReleased);
                var next = baslineList.OrderBy(b => b.ReleaseDate).FirstOrDefault(s => !s.IsReleased);
                var info = new StringBuilder();

                if (next != null)
                {
                    int days = next.ReleaseDate.Subtract(DateTime.Today).Days;
                    string nextStatus = days < 0
                        ? "<span class='text-danger'>Delayed " + Math.Abs(days) + " day(s)</span>"
                        : "<span class='text-warning'>" + days + " day(s) left</span>";
                    info.Append("<h5 style='word-wrap: break-word'><b>Next:</b> " + next.Name + " (" + nextStatus +
                                ") </h5>");
                }

                if (last != null)
                {
                    info.Append("<h5 style='word-wrap: break-word'><b>Last:</b> " + last.Name + " (" +
                                last.ReleaseDate.ToString("yyyy-M-d") + ")</h5>");
                }

                if (pendingAudit != null)
                {
                    string auditStatus = pendingAudit != null
                        ? "<span class='text-danger'>Pending baseline audit: " + pendingAudit.BaselinePlan.Name +
                          "</span>"
                        : "Audits on time.";
                    info.Append("<h5 style='word-wrap: break-word'><b>" + auditStatus + "</b></h5>");
                }

                var template = string.Format(PanelDashboardTemplate(), "<img src='../../Images/baselineIcon.png' width=30px height:30px></img> Baselines", info);
                htmlPanel += template;
            }
        }

        private void InsertAuditsPanel(ref string htmlPanel, int projectId)
        {
            var auditList = db.Audits.Where(t => t.ProjectId == projectId);
            if (auditList.Any())
            {
                var last = auditList.OrderBy(a => a.ScheduledDate).FirstOrDefault(x => x.PerformedDate != null);
                var next = auditList.OrderBy(a => a.ScheduledDate).FirstOrDefault(x => x.PerformedDate == null);

                string lastStatus = last != null ? last.PerformedDate.Value.ToString("yyyy-M-d") : "";
                string nextStatus = "";
                if (next != null)
                {
                    nextStatus = next.ScheduledDate.ToString("yyyy-M-d");
                    int days = next.ScheduledDate.Subtract(DateTime.Today).Days;
                    nextStatus += " <b>(" + days + " day(s) left)</b>";
                }

                var template = string.Format(PanelDashboardTemplate(), "<img src='../../Images/auditIcon.png' width=20px height:25px></img> Audits",
                    "<h5 style='word-wrap: break-word'><b>Last Performed:</b> " + lastStatus + "</h5>" +
                    "<h5 style='word-wrap: break-word'><b>Next Scheduled:</b> " + nextStatus + "</h5>");
                htmlPanel += template;
            }
        }

        private string PanelDashboardTemplate()
        {
            return @"<div class='col-sm-4'>" + 
                "<div class='panel panel-default'>" +
                    "<div class='panel-heading'>" +
                        "<h3 class='panel-title'>{0}</h3>" +
                        "<span class='pull-right clickable'><i class='glyphicon glyphicon-chevron-up'></i></span>" +
                    "</div>" +
                    "<div class='panel-body'>" +
                        "{1}" +
                    "</div>" +
                "</div>" +
            "</div>";
        }
    }
}