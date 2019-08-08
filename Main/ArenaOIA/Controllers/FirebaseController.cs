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

                if(submission == null)
                {
                    submission = new Submission();
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
                Contest oldContest = GetContest(contest.Id);
                contest.Participantes = oldContest.Participantes;

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
    }
}