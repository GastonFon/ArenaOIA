using ArenaOIA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArenaOIA.Controllers
{
    public class ArenaController : Controller
    {
        private HelperController helper = new HelperController();
        private APIController api = new APIController();
        private FirebaseController firebase = new FirebaseController();
        // GET: Arena
        public ActionResult Contest(string Id)
        {
            if (TempData["errorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["error"];
            }

            if (Session["username"] != null)
            {
                ViewBag.LoggedIn = true;
                ViewBag.Username = Session["username"].ToString();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            string username = Session["username"].ToString();

            if (helper.ContestRunning(Id) == false)
            {
                return RedirectToAction("Index", "Home");
            }

            if (helper.UserRegisteredToContest(Id, username) == false)
            {
                return RedirectToAction("Index", "Home");
            }

            ///Contest is running and user is registered

            Contest contest = firebase.GetContest(Id);

            int totalScore = 0;

            Dictionary<string, int> scores = new Dictionary<string, int>();
            var bestSubmissions = firebase.BestSubmissionsUser(Id, username);

            foreach (var problema in contest.Problemas)
            {
                try
                {
                    int bestScore = bestSubmissions[problema.Nombre].Puntaje;
                    scores[problema.Nombre] = bestScore;
                    totalScore += bestScore;
                }
                catch
                {
                    scores[problema.Nombre] = -1;
                }
            }

            ViewBag.TotalScore = totalScore;
            ViewBag.Scores = scores;

            return View(contest);
        }
        public ActionResult Problem(string Id, string problem)
        {
            if (TempData["errorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["error"];
            }

            if (Session["username"] != null)
            {
                ViewBag.LoggedIn = true;
                ViewBag.Username = Session["username"].ToString();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            string username = Session["username"].ToString();

            if (helper.ContestRunning(Id) == false)
            {
                return RedirectToAction("Index", "Home");
            }

            if (helper.UserRegisteredToContest(Id, username) == false)
            {
                return RedirectToAction("Index", "Home");
            }

            ///Contest is running and user is registered

            Contest cvm = firebase.GetContest(Id);

            bool encontrado = false;
            int puntaje = 0;
            foreach (var item in cvm.Problemas)
            {
                if (item.Nombre == problem)
                {
                    puntaje = item.Puntaje;
                    encontrado = true;
                }
            }
            if (encontrado == false)
            {
                TempData["errorMessage"] = "Problema no encontrado.";
                return RedirectToAction("Contest", new { Id });
            }

            string error = "";
            ProblemViewModel pvm = api.GetProblem(problem, username, Id, ref error);
            pvm.MaxScore = puntaje;

            if (error != "")
            {
                TempData["errorMessage"] = error;
                return RedirectToAction("Contest", new { Id });
            }

            ViewBag.ContestId = Id;
            ViewBag.Problema = problem;
            ViewBag.BestScore = firebase.GetBestSubmission(Id, username, problem).Puntaje;

            return View(pvm);
        }

        [HttpPost]
        public ActionResult Problem(string Id, string problem, string code)
        {
            ///JS btoa(unescape(encodeURIComponent(textArea)))
            if (Session["username"] != null)
            {
                ViewBag.LoggedIn = true;
                ViewBag.Username = Session["username"].ToString();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            string username = Session["username"].ToString();

            if (helper.ContestRunning(Id) == false)
            {
                return RedirectToAction("Index", "Home");
            }

            if (helper.UserRegisteredToContest(Id, username) == false)
            {
                return RedirectToAction("Index", "Home");
            }

            ///Contest is running and user is registered

            Contest cvm = firebase.GetContest(Id);

            bool encontrado = false;
            int puntaje = 0;
            foreach (var item in cvm.Problemas)
            {
                if (item.Nombre == problem)
                {
                    puntaje = item.Puntaje;
                    encontrado = true;
                }
            }
            if (encontrado == false)
            {
                return RedirectToAction("Contest", new { Id });
            }

            ///Problem exists
            string error = "";
            Submission submission = api.Submit(problem, username, code, Session["token"].ToString(), Id, puntaje, ref error);

            if (error != "")
            {
                TempData["errorMessage"] = error;
                return RedirectToAction("Problem", new { Id, problem });
            }

            firebase.SetSubmission(submission);
            firebase.UpdateBestSubmission(submission);

            return RedirectToAction("Problem", new { Id, problem });
        }

        public ActionResult Ranking(string Id)
        {
            string username = "";
            if (Session["username"] != null)
            {
                ViewBag.LoggedIn = true;
                ViewBag.Username = Session["username"].ToString();
                username = Session["username"].ToString();
            }
            else
            {
                ViewBag.LoggedIn = false;
            }

            bool isRunning = helper.ContestRunning(Id);
            bool isAdmin = helper.UserIsAdmin(username);
            Contest contest = firebase.GetContest(Id);

            if (contest.Ranking == false || (isRunning && !isAdmin))
            {
                return RedirectToAction("Index", "Home");
            }

            ///Contest is not running, or
            ///Contest is running and user is admin

            var bestSubmissions = firebase.BestSubmissionsContest(Id);

            string table = "<table class=\"table table-hover table-dark\" id =\"leaderboard\">";
            table += "<tr>";
            table += "<th>Participante</th>";
            table += "<th>Total</th>";
            foreach (var problema in contest.Problemas)
            {
                table += "<th>" + problema.Nombre + "</th>";
            }
            table += "</tr>";

            foreach (var participante in contest.Participantes)
            {
                int sumaTotal = 0;
                table += "<tr><td>" + participante + "</td>";
                string row = "";
                foreach (var problema in contest.Problemas)
                {
                    try
                    {
                        int bestScore = bestSubmissions[participante][problema.Nombre].Puntaje;
                        row += "<td>" + bestScore + "</td>";
                        sumaTotal += bestScore;
                    }
                    catch
                    {
                        row += "<td>NO</td>";
                    }
                }
                table += "<td>" + sumaTotal + "</td>";
                table += row + "</tr>";
            }

            table += "</table>";

            ViewBag.table = table;
            ViewBag.nombreContest = contest.Id;

            return View();
        }
    }
}