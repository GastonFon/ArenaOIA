using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArenaOIA.Models;

namespace ArenaOIA.Controllers
{
    public class HelperController : Controller
    {
        private FirebaseController firebase = new FirebaseController();
        // GET: Helper
        public ActionResult Index()
        {
            return View();
        }

        public bool UserIsAdmin(string username)
        {
            if (username == null)
                return false;
            return (username == "GastonFontenla");
        }

        public DateTime HoraServerABsAs(DateTime time)
        {
            return time.AddHours(-3);
        }

        public DateTime HoraActualBsAs()
        {
            return HoraServerABsAs(DateTime.Now);
        }
        /**
        public ContestViewModel EntityToViewModel(Contest contest)
        {
            if (contest == null)
            {
                return null;
            }
            ContestViewModel cvm = new ContestViewModel();
            cvm.Id = contest.Id;
            cvm.Nombre = contest.Nombre;
            cvm.Inicio = contest.Inicio;
            cvm.Fin = contest.Fin;
            cvm.Problemas = new List<Problem>();
            List<string> problemas = contest.Problemas.Split(';').ToList();
            int i = 0;
            foreach (var problema in problemas)
            {
                List<string> datos = problema.Split('?').ToList();
                Problem p = new Problem();
                p.Id = i++;
                p.Nombre = datos[0];
                p.Puntaje = Int32.Parse(datos[1]);
                cvm.Problemas.Add(p);
            }
            cvm.Participantes = new List<string>();
            if (contest.Participantes != null)
                cvm.Participantes = contest.Participantes.Split(';').ToList();

            return cvm;
        }

        public Contest ViewModelToEntity(ContestViewModel cvm)
        {
            if (cvm == null)
            {
                return null;
            }
            Contest contest = db.Contests.Find(cvm.Id);

            if (contest == null)
            {
                contest = new Contest();
            }
            contest.Nombre = cvm.Nombre;
            contest.Inicio = cvm.Inicio;
            contest.Fin = cvm.Fin;
            contest.Problemas = "";
            bool primer = true;
            foreach (var problema in cvm.Problemas)
            {
                if (primer == false)
                    contest.Problemas += ";";

                contest.Problemas += String.Format("{0}?{1}", problema.Nombre, problema.Puntaje);

                primer = false;
            }

            return contest;
        }
    */

        public bool UserRegisteredToContest(string Id, string username)
        {
            //Contest contest = db.Contests.Find(Id);

            Contest contest = firebase.GetContest(Id);

            if (username == null || username == "" || contest == null || contest.Participantes == null || contest.Participantes.Count() == 0)
                return false;

            foreach(var participante in contest.Participantes)
            {
                if(participante == username)
                {
                    return true;
                }
            }

            return false;
        }

        public bool ContestRunning(string Id)
        {
            //Contest contest = db.Contests.Find(Id);

            Contest contest = firebase.GetContest(Id);

            if (contest == null)
                return false;

            DateTime ahora = HoraActualBsAs();

            return (contest.Inicio <= ahora && ahora <= contest.Fin);
        }

        public int BestScore(string problema, string username, string contestId, int maxScore)
        {
            int bestScore = firebase.GetBestSubmission(contestId, username, problema).Puntaje;
            
            ///100% -> maxScore
            ///bestScore -> ?

            return (bestScore*maxScore)/100;
        }
    }
}