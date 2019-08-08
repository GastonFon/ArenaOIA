using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ArenaOIA.Models;

namespace ArenaOIA.Controllers
{
    class Autorizado : FilterAttribute, IAuthorizationFilter
    {
        private HelperController helper = new HelperController();
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["username"] == null)
            {
                filterContext.Result = new RedirectResult("~/Home/Index/");
            }
            else if(helper.UserIsAdmin(filterContext.HttpContext.Session["username"].ToString()) == false)
            {
                filterContext.Result = new RedirectResult("~/Home/Index/");
            }
        }
    }

    [Autorizado]
    public class ContestsController : Controller
    {
        private HelperController helper = new HelperController();
        private FirebaseController firebase = new FirebaseController();
        private static int contador = 0;

        // GET: Contests
        public ActionResult Index()
        {
            if (Session["username"] != null)
            {
                ViewBag.LoggedIn = true;
                ViewBag.Username = Session["username"].ToString();
            }

            return View(firebase.GetContests());
        }

        // GET: Contests/Details/5
        public ActionResult Details(string id)
        {
            if (Session["username"] != null)
            {
                ViewBag.LoggedIn = true;
                ViewBag.Username = Session["username"].ToString();
            }

            if ( id == null || id == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Contest contest = firebase.GetContest(id);

            if (contest == null)
            {
                return HttpNotFound();
            }
            return View(contest);
        }

        // GET: Contests/Create
        public ActionResult Create()
        {
            if (Session["username"] != null)
            {
                ViewBag.LoggedIn = true;
                ViewBag.Username = Session["username"].ToString();
            }

            Contest contest = new Contest();
            return View(contest);
        }

        // POST: Contests/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contest contest)
        {
            if (Session["username"] != null)
            {
                ViewBag.LoggedIn = true;
                ViewBag.Username = Session["username"].ToString();
            }

            if (ModelState.IsValid)
            {
                firebase.SetContest(contest);
                return RedirectToAction("Index");
            }

            return View(contest);
        }

        // GET: Contests/Edit/5
        public ActionResult Edit(string id)
        {
            if (Session["username"] != null)
            {
                ViewBag.LoggedIn = true;
                ViewBag.Username = Session["username"].ToString();
            }

            if (id == null || id == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contest contest = firebase.GetContest(id);
            if (contest == null)
            {
                return HttpNotFound();
            }
            return View(contest);
        }

        // POST: Contests/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Contest contest)
        {
            if (Session["username"] != null)
            {
                ViewBag.LoggedIn = true;
                ViewBag.Username = Session["username"].ToString();
            }

            if (ModelState.IsValid)
            {
                firebase.UpdateContest(contest);
                return RedirectToAction("Index");
            }
            return View(contest);
        }

        // GET: Contests/Delete/5
        public ActionResult Delete(string id)
        {
            if (Session["username"] != null)
            {
                ViewBag.LoggedIn = true;
                ViewBag.Username = Session["username"].ToString();
            }

            if (id == null || id == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contest contest = firebase.GetContest(id);
            if (contest == null)
            {
                return HttpNotFound();
            }
            return View(contest);
        }

        // POST: Contests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if (Session["username"] != null)
            {
                ViewBag.LoggedIn = true;
                ViewBag.Username = Session["username"].ToString();
            }

            firebase.DeleteContest(id);
            return RedirectToAction("Index");
        }

        public PartialViewResult BlankEditorRow()
        {
            Problem p = new Problem();
            p.Id = contador++;

            return PartialView("ProblemEditorRow", p);
        }

        public PartialViewResult EditorRow(Problem problema)
        {
            return PartialView("ProblemEditorRow", problema);
        }
    }
}
