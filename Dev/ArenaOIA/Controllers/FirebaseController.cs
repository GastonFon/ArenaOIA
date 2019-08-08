using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArenaOIA.Models;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;

namespace ArenaOIA.Controllers
{
    public class FirebaseController : Controller
    {
        readonly IFirebaseConfig config = new FirebaseConfig
        {

        };

        IFirebaseClient client;

        public void SetSubmission(Submission s)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                string documentName = "Submissions/" + s.ContestId.ToString() + "/" + s.Username + "/" + s.ProblemName + "/" + s.OIAJSubmissionId;
                client.Set(documentName, s);
            }
            catch (Exception e)
            {
                throw new NotImplementedException(e.ToString());
            }

            AddToLatestSubmissions(s);
        }

        public List<Submission> GetSubmissions(string contestId, string username, string problem)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Submissions/" + contestId + "/" + username + "/" + problem);
                var resultados = response.ResultAs<IDictionary<string, Submission>>().Values.ToList();
                return resultados;
            }
            catch (Exception e)
            {
                return new List<Submission>();
            }
        }

        public Submission GetBestSubmission(string contestId, string username, string problem)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("BestSubmissions/" + contestId + "/" + username + "/" + problem);
                Submission submission = response.ResultAs<Submission>();

                if (submission == null)
                {
                    submission = new Submission();
                    submission.Puntaje = -1;
                }

                return submission;
            }
            catch (Exception e)
            {
                return new Submission();
            }
        }

        public void UpdateBestSubmission(Submission s)
        {
            Submission submission = GetBestSubmission(s.ContestId.ToString(), s.Username, s.ProblemName);
            if (submission.Puntaje <= s.Puntaje)
            {
                try
                {
                    string documentName = "BestSubmissions/" + s.ContestId.ToString() + "/" + s.Username + "/" + s.ProblemName;
                    client.Update(documentName, s);
                }
                catch (Exception e)
                {
                    throw new NotImplementedException(e.ToString());
                }
            }
        }

        public Contest GetContest(string contestId)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Contests/" + contestId);
                Contest contest = response.ResultAs<Contest>();

                if (contest == null)
                    return null;

                if (contest.Participantes == null)
                    contest.Participantes = new List<string>();

                if (contest.Problemas == null)
                    contest.Problemas = new List<Problem>();

                return contest;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<Contest> GetContests()
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Contests");

                List<Contest> lista = response.ResultAs<IDictionary<string, Contest>>().Values.ToList();
                List<Contest> listaCurada = new List<Contest>();

                foreach (Contest contest in lista)
                {
                    if (contest.Participantes == null)
                        contest.Participantes = new List<string>();

                    if (contest.Problemas == null)
                        contest.Problemas = new List<Problem>();

                    listaCurada.Add(contest);
                }

                return listaCurada;
            }
            catch (Exception e)
            {
                return new List<Contest>();
            }
        }

        public bool SetContest(Contest contest)
        {
            try
            {
                if (contest.Participantes == null)
                    contest.Participantes = new List<string>();
                contest.Participantes.Add("unlam_01");
                client = new FireSharp.FirebaseClient(config);
                string documentName = "Contests/" + contest.Id;
                client.Set(documentName, contest);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool UpdateContest(Contest contest)
        {
            try
            {
                if (contest.Participantes == null)
                {
                    contest.Participantes.Add("unlam_01");
                }

                string documentName = "Contests/" + contest.Id;
                client.Update(documentName, contest);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool DeleteContest(string contestId)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                client.Delete("Contests/" + contestId);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public IDictionary<string, IDictionary<string, Submission>> BestSubmissionsContest(string contestId)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("BestSubmissions/" + contestId);
                var lista = response.ResultAs<IDictionary<string, IDictionary<string, Submission>>>();
                return lista;
            }
            catch
            {
                return null;
            }
        }

        public IDictionary<string, Submission> BestSubmissionsUser(string contestId, string username)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("BestSubmissions/" + contestId + "/" + username);
                var lista = response.ResultAs<IDictionary<string, Submission>>();
                return lista;
            }
            catch
            {
                return null;
            }
        }

        public void AddToLatestSubmissions(Submission s)
        {
            ///client is already initialized
            try
            {
                string documentName = "LatestSubmissions/" + s.Fecha.Year + "/" + s.Fecha.Month + "/" + s.OIAJSubmissionId;
                client.Set(documentName, s);
            }
            catch (Exception e)
            {
                throw new NotImplementedException(e.ToString());
            }
        }

        public List<Submission> LatestSubmissions(DateTime fecha)
        {
            try
            {
                ///Last two months submissions
                client = new FireSharp.FirebaseClient(config);

                FirebaseResponse response = client.Get("LatestSubmissions/" + fecha.Year + "/" + fecha.Month + "/");
                List<Submission> submissions1 = new List<Submission>();

                try
                {
                    submissions1 = response.ResultAs<IDictionary<string, Submission>>().Values.ToList();
                }
                catch
                {

                }

                fecha = fecha.AddMonths(-1);

                response = client.Get("LatestSubmissions/" + fecha.Year + "/" + fecha.Month + "/");
                List<Submission> submissions2 = new List<Submission>();

                try
                {
                    submissions2 = response.ResultAs<IDictionary<string, Submission>>().Values.ToList();
                }
                catch
                {

                }

                List<Submission> submissions = new List<Submission>();

                if (submissions1 != null)
                    submissions.AddRange(submissions1);
                if (submissions2 != null)
                    submissions.AddRange(submissions2);

                submissions = submissions.OrderByDescending(o => o.Fecha).ToList();

                return submissions;
            }
            catch (Exception e)
            {
                throw new NotImplementedException(e.ToString());
            }
        }
    }
}