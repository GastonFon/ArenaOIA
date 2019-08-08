using ArenaOIA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArenaOIA.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private APIController api = new APIController();
        private HelperController helper = new HelperController();
        private FirebaseController firebase = new FirebaseController();

        public ActionResult Index()
        {
            if (Session["username"] != null)
            {
                ViewBag.LoggedIn = true;
                ViewBag.Username = Session["username"].ToString();
            }

            ViewBag.CurrentDateTime = helper.HoraActualBsAs();

            return View(firebase.GetContests());
        }

        public ActionResult Login()
        {
            if (Session["username"] != null)
            {
                ViewBag.LoggedIn = true;
                ViewBag.Username = Session["username"].ToString();
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            string token = api.OIAJLogin(login.Username, login.Password);

            if (token == "")
            {
                ViewBag.ErrorMessage = "Datos incorrectos";
                return View();
            }

            Session["username"] = login.Username;
            Session["password"] = login.Password;
            Session["token"] = token;

            ViewBag.LoggedIn = true;
            ViewBag.Username = login.Username;

            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            Session.RemoveAll();

            return RedirectToAction("Index");
        }

        public ActionResult Register(string Id)
        {
            if (Session["username"] != null)
            {
                ViewBag.LoggedIn = true;
                ViewBag.Username = Session["username"].ToString();
            }
            else
            {
                return RedirectToAction("Login");
            }

            string username = Session["username"].ToString();

            if (helper.UserRegisteredToContest(Id, username) == false)
            {
                Contest contest = firebase.GetContest(Id);
                contest.Participantes.Add(username);
                firebase.UpdateContest(contest);
            }

            return RedirectToAction("Index");
        }
    }
}