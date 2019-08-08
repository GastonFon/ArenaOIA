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

            int puntajeTotal = 0;

            foreach(var problema in contest.Problemas)
            {
                puntajeTotal += helper.BestScore(problema.Nombre, username, Id, problema.Puntaje);
            }

            ViewBag.TotalScore = puntajeTotal;

            return View(contest);
        }
        public ActionResult Problem(string Id, string problem)
        {
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

            ProblemViewModel pvm = api.GetProblem(problem, username, Id, cvm.Inicio, cvm.Fin);

            ViewBag.ContestId = Id;
            ViewBag.Problema = problem;
            ViewBag.BestScore = helper.BestScore(problem, username, Id, puntaje);
            ViewBag.Puntaje = puntaje;

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
            ///string problema, string username, string code, string token, int contestId
            Submission submission = api.Submit(problem, username, code, Session["token"].ToString(), Id, puntaje);

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


            if (helper.ContestRunning(Id) == true && helper.UserIsAdmin(username) == false)
            {
                return RedirectToAction("Index", "Home");
            }

            ///Contest is not running, or
            ///Contest is running and user is admin

            Contest contest = firebase.GetContest(Id);

            string table = "<table class=\"table thead-dark table-bordered\" id =\"leaderboard\">";
            table += "<tr>";
            table += "<th>Participante</th>";
            table += "<th>Total</th>";
            foreach(var problema in contest.Problemas)
            {
                table += "<th>" + problema.Nombre + "</th>";
            }
            table += "</tr>";

            foreach(var participante in contest.Participantes)
            {
                int sumaTotal = 0;
                table += "<tr><td>" + participante + "</td>";
                string row = "";
                foreach(var problema in contest.Problemas)
                {
                    int bestScore = helper.BestScore(problema.Nombre, participante, Id, problema.Puntaje);
                    row += "<td>" + bestScore + "</td>";
                    sumaTotal += bestScore;
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