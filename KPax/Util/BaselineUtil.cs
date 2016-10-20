using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using KPax.DataModels;
using KPax.Models;

namespace KPax.Util
{
    public class BaselineUtil
    {
        public static string ApplyTag(int projectId, string tag)
        {
            try
            {
                var gitSetupDB = new EntitiesDB().GitSetup;
                var gitModel = gitSetupDB.FirstOrDefault(i => i.ProjectId == projectId);
                if (gitModel == null)
                {
                    return "There is no Git setting for this project.";
                }
                string credential = new Crypt32().Decrypt(gitModel.Crendential);
                var status = new GitSharpClient(gitModel.RepositoryPath).Tag(tag, credential.Split('|')[0], credential.Split('|')[1]);
                if (status)
                {
                    return "Tag " + tag + "was applied with success.";// "Git tag command process with success. ";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "Error on tagging baseline. Verify Git setup.";
        }

        public static string BuildTeamCity(int projectId)
        {
            try
            {
                var teamCityDb = new EntitiesDB().TeamCitySetup;
                var teamCityModel = teamCityDb.FirstOrDefault(t => t.IdProject == projectId);
                if (teamCityModel == null)
                {
                    return "There is no TeamCity setting for this project.";
                }
                var userName = new Crypt32().Decrypt(teamCityModel.Crendential).Split(';')[0];
                var password = new Crypt32().Decrypt(teamCityModel.Crendential).Split(';')[1];

                var request = (HttpWebRequest) WebRequest.Create(teamCityModel.Uri.Trim() + "/httpAuth/app/rest/buildQueue");
                request.Method = "POST";
                request.ContentType = "application/xml";
                request.Accept = "application/xml";
                String encoded =
                    System.Convert.ToBase64String(
                        System.Text.Encoding.GetEncoding("ISO-8859-1")
                            .GetBytes(userName + ":" + password));
                request.Headers.Add("Authorization", "Basic " + encoded);

                string requestXml = @"<build><buildType id=""" + teamCityModel.BuildId.Trim() + "\"/></build>";
                var xmlBytes = System.Text.Encoding.UTF8.GetBytes(requestXml); ;
                request.ContentLength = xmlBytes.Length;
                using (var reqStream = request.GetRequestStream())
                {
                    reqStream.Write(xmlBytes, 0, xmlBytes.Length);
                }

                string buildId = "";
                using (WebResponse response = request.GetResponse())
                {
                    if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                    {
                        Stream responseStream = response.GetResponseStream();
                        string responseStr = new StreamReader(responseStream).ReadToEnd();
                        var set = new XmlReaderSettings();
                        set.ConformanceLevel = ConformanceLevel.Fragment;
                        var doc = new XPathDocument(XmlReader.Create(new StringReader(responseStr), set));

                        XPathNavigator nav = doc.CreateNavigator();
                        buildId = nav.SelectSingleNode("build").GetAttribute("id", "");
                    }
                }
                
                var tcBuildTO = GetBuildStatus(buildId, teamCityModel);
                while (tcBuildTO.State != "finished")
                {
                    Thread.Sleep(2000);
                    tcBuildTO = GetBuildStatus(buildId, teamCityModel);
                    if (tcBuildTO == null)
                    {
                        break;
                    }
                }

                return tcBuildTO.Status + " Build: " + tcBuildTO.Number + " Started at: " + tcBuildTO.StartDate + " - " + tcBuildTO.StatusText;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string GetTeamCityLastBuildId(TeamCitySetup teamCityModel)
        {
            // http://10.101.48.18:8111/httpAuth/app/rest/builds/?locator=project:TaggedForms
            try
            {
                var userName = new Crypt32().Decrypt(teamCityModel.Crendential).Split(';')[0];
                var password = new Crypt32().Decrypt(teamCityModel.Crendential).Split(';')[1];

                var tcProjectId = teamCityModel.BuildId.Split('_')[0];

                var request = (HttpWebRequest)WebRequest.Create(teamCityModel.Uri.Trim() + "/httpAuth/app/rest/builds/?locator=project:" + tcProjectId);
                request.Method = "GET";
                request.ContentType = "application/xml";
                request.Accept = "application/xml";
                String encoded =
                    System.Convert.ToBase64String(
                        System.Text.Encoding.GetEncoding("ISO-8859-1")
                            .GetBytes(userName + ":" + password));
                request.Headers.Add("Authorization", "Basic " + encoded);

                string buildId = "";
                using (WebResponse response = request.GetResponse())
                {
                    if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                    {
                        Stream responseStream = response.GetResponseStream();
                        string responseStr = new StreamReader(responseStream).ReadToEnd();
                        var set = new XmlReaderSettings();
                        set.ConformanceLevel = ConformanceLevel.Fragment;
                        var doc = new XPathDocument(XmlReader.Create(new StringReader(responseStr), set));

                        XPathNavigator nav = doc.CreateNavigator();
                        var node = nav.Select("builds/build");
                        if (node.MoveNext())
                        {
                            return node.Current.GetAttribute("id", "");
                        }
                    }
                }

                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static TeamCityBuildTO GetBuildStatus(string buildId, TeamCitySetup teamCityModel)
        {
            /* GET /httpAuth/app/rest/buildQueue/id:135763 */

            var userName = new Crypt32().Decrypt(teamCityModel.Crendential).Split(';')[0];
            var password = new Crypt32().Decrypt(teamCityModel.Crendential).Split(';')[1];
            var request = (HttpWebRequest)WebRequest.Create(teamCityModel.Uri.Trim() + "/httpAuth/app/rest/buildQueue/id:" + buildId);
            String encoded =
                System.Convert.ToBase64String(
                    System.Text.Encoding.GetEncoding("ISO-8859-1")
                        .GetBytes(userName + ":" + password));
            request.Headers.Add("Authorization", "Basic " + encoded);

            using (WebResponse response = request.GetResponse())
            {
                if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    string responseStr = new StreamReader(responseStream).ReadToEnd();

                    var set = new XmlReaderSettings();
                    set.ConformanceLevel = ConformanceLevel.Fragment;
                    var doc = new XPathDocument(XmlReader.Create(new StringReader(responseStr), set));

                    XPathNavigator nav = doc.CreateNavigator();
                    var tcBuildTO = new TeamCityBuildTO();
                    tcBuildTO.State = nav.SelectSingleNode("build").GetAttribute("state", "");
                    tcBuildTO.Status = nav.SelectSingleNode("build").GetAttribute("status", "");

                    if ((tcBuildTO.State == "finished"))
                    {
                        tcBuildTO.Number = int.Parse(nav.SelectSingleNode("build").GetAttribute("number", ""));
                        tcBuildTO.StatusText = nav.SelectSingleNode("build/statusText").ToString();
                        tcBuildTO.StartDate = DateTime.ParseExact(nav.SelectSingleNode("build/startDate").ToString(),
                            "yyyyMMdd'T'HHmmss-ffff", CultureInfo.InvariantCulture);
                    }
                    // 2016 08 24 T 15 09 10 -0300
                    return tcBuildTO;
                }
            }

            return null;
        }

        public static List<string> GetBuildIds(TeamCityViewModel teamCityViewModel)
        {
            /* GET /httpAuth/app/rest/buildQueue/id:135763 */
            var list = new List<string>();  
            var request = (HttpWebRequest)WebRequest.Create(teamCityViewModel.URI.Trim() + "/httpAuth/app/rest/buildTypes");
            String encoded =
                System.Convert.ToBase64String(
                    System.Text.Encoding.GetEncoding("ISO-8859-1")
                        .GetBytes(teamCityViewModel.UserName + ":" + teamCityViewModel.Password));
            request.Headers.Add("Authorization", "Basic " + encoded);

            using (WebResponse response = request.GetResponse())
            {
                if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    string responseStr = new StreamReader(responseStream).ReadToEnd();

                    var set = new XmlReaderSettings();
                    set.ConformanceLevel = ConformanceLevel.Fragment;
                    var doc = new XPathDocument(XmlReader.Create(new StringReader(responseStr), set));

                    XPathNavigator nav = doc.CreateNavigator();

                    XPathNodeIterator nodes = nav.Select("buildTypes/buildType");
                    while (nodes.MoveNext())
                    {
                        list.Add(nodes.Current.GetAttribute("id", ""));
                    }
                }
            }

            return list;
        }

        public static string SendEmail(int projectId, string tag, string userName)
        {
            var projDb = new EntitiesDB().Project;
            var projModel = projDb.FirstOrDefault(p => p.Id == projectId);

            try
            {
                //kpaxatlantico@gmail.com
                //123qwe!@#QWE
                var fromAddress = new MailAddress("kpaxatlantico@gmail.com", "K-Pax");
                var toAddress = new MailAddress(projModel.Email, projModel.Name);
                const string fromPassword = "123qwe!@#QWE";
                string subject = "Baseline " + tag;
                string body = projModel.TemplateEmail.Replace("{sprint}", tag);

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception e)
            {
                return "Some problem when sending the baseline e-mail. " + e.Message;
            }

            return "Sent";
        }
    }
}