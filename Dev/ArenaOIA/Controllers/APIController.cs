using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Net;
using ArenaOIA.Models;
using System.Text.RegularExpressions;

namespace ArenaOIA.Controllers
{
    public class APIController : Controller
    {
        private HelperController helper = new HelperController();
        private FirebaseController firebase = new FirebaseController();
        // GET: API
        public ActionResult Index()
        {
            return View();
        }
        public string OIAJLogin(string username, string password, ref string error)
        {
            ///Intenta loguearse en OIAJ
            ///Si no lo logra, devuelve ""

            var client = new RestClient("http://juez.oia.unsam.edu.ar/api/user");
            var request = new RestRequest(Method.POST);
            request.AddHeader("postman-token", "7feab15c-13b6-5ffa-b359-3a85761d3a8e");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\"action\":\"login\",\"username\":\"" + username + "\",\"password\":\"" + password + "\"}\r\n", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            JObject respuesta = new JObject();
            try
            {
                respuesta = JObject.Parse(response.Content);
                while (respuesta.Count == 0 || respuesta.GetValue("success") == null)
                {
                    System.Threading.Thread.Sleep(1000);
                    response = client.Execute(request);
                    respuesta = JObject.Parse(response.Content);
                }
            }
            catch (Exception e)
            {
                error = "Hubo un error en el servidor. Inténtelo nuevamente.";
                return "";
            }

            if (respuesta.GetValue("success").ToString() == "0")
            {
                error = "Hubo un error en el servidor. Inténtelo nuevamente.";
                return "";
            }

            string token = response.Cookies[1].Value;

            return token;
        }

        public ProblemViewModel GetProblem(string problema, string username, string contestId, ref string error)
        {
            var client = new RestClient("http://juez.oia.unsam.edu.ar/api/task");
            var request = new RestRequest(Method.POST);
            request.AddHeader("postman-token", "be07341a-d5e3-7184-0d50-cccc1b802cc5");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json;charset=UTF-8");
            request.AddParameter("application/json;charset=UTF-8", "{\"name\":\"" + problema + "\",\"action\":\"get\"}\r\n", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            JObject respuesta = new JObject();

            int maxAttempts = 10;
            while (maxAttempts-- > 0)
            {
                try
                {
                    respuesta = JObject.Parse(response.Content);
                    while (respuesta.Count == 0 || respuesta.GetValue("success") == null)
                    {
                        System.Threading.Thread.Sleep(1000);
                        response = client.Execute(request);
                        respuesta = JObject.Parse(response.Content);
                    }
                    break;
                }
                catch (Exception e)
                {
                    System.Threading.Thread.Sleep(1000);
                    response = client.Execute(request);
                    //throw new NotImplementedException(e.ToString());
                }
            }

            if(maxAttempts == 0)
            {
                error = "No se pudo cargar el problema.";
                return null;
            }

            if (respuesta.GetValue("success").ToString() == "0")
            {
                error = "No se pudo cargar el problema.";
                return null;
            }
            ProblemViewModel pvm = new ProblemViewModel();

            pvm.Nombre = respuesta.GetValue("title").ToString();
            pvm.LimiteMemoria = respuesta.GetValue("memory_limit").ToString();
            pvm.LimiteTiempo = respuesta.GetValue("time_limit").ToString();
            pvm.Envios = firebase.GetSubmissions(contestId, username, problema);
            pvm.Envios.Reverse();
            pvm.Archivos = new List<Tuple<string, string> >();

            JObject enunciado = JObject.Parse(respuesta.GetValue("statements").ToString());
            pvm.Archivos.Add(Tuple.Create(problema + ".pdf", enunciado.GetValue("es").ToString()));

            if (respuesta.GetValue("attachments") != null)
            {
                var attachments = respuesta.GetValue("attachments").Children();
                foreach (var item in attachments)
                {
                    var nombreArchivo = item[0].ToString();
                    var tokenArchivo = item[1].ToString();
                    pvm.Archivos.Add(Tuple.Create(nombreArchivo, tokenArchivo));
                }
            }

            return pvm;
        }

        public string GetScore(string OIAJSubmissionId, string token, ref JObject json)
        {
            var client = new RestClient("http://juez.oia.unsam.edu.ar/api/submission");
            var request = new RestRequest(Method.POST);
            request.AddCookie("token", token);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\"action\":\"details\",\"id\":\"" + OIAJSubmissionId + "\"}\r\n", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            JObject respuesta = new JObject();

            int maxAttempts = 10;
            while (maxAttempts-- > 0)
            {
                try
                {
                    respuesta = JObject.Parse(response.Content);
                    while (respuesta.Count == 0 || respuesta.GetValue("score") == null)
                    {
                        System.Threading.Thread.Sleep(1000);
                        response = client.Execute(request);
                        respuesta = JObject.Parse(response.Content);
                    }
                    break;
                }
                catch (Exception e)
                {
                    System.Threading.Thread.Sleep(1000);
                    response = client.Execute(request);
                    //throw new NotImplementedException(e.ToString());
                }
            }

            json = respuesta;

            return respuesta.GetValue("score").ToString();
        }

        public Submission Submit(string problema, string username, string code, string token, string contestId, int maxScore, ref string error)
        {
            code = Regex.Replace(code, @"\s+", "+");

            DateTime localSubmissionTime = helper.HoraActualBsAs();

            Submission submission = new Submission();
            string formato = "{\"files\":{\"" + problema +".%l\":{\"filename\":\"ace.cpp\",\"data\":\"" + code + "\"}},\"action\":\"new\",\"task_name\":\"" + problema + "\"}";
            var client = new RestClient("http://juez.oia.unsam.edu.ar/api/submission");
            var request = new RestRequest(Method.POST);
            request.AddHeader("postman-token", "043817e3-b128-8680-1a21-be95fc74728c");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddCookie("token", token);
            request.AddParameter("application/json", formato, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            JObject respuesta = new JObject();

            int maxAttempts = 10;
            while (maxAttempts-- > 0)
            {
                try
                {
                    respuesta = JObject.Parse(response.Content);
                    while (respuesta.Count == 0 || respuesta.GetValue("success") == null)
                    {
                        System.Threading.Thread.Sleep(1000);
                        response = client.Execute(request);
                        respuesta = JObject.Parse(response.Content);
                    }
                    break;
                }
                catch (Exception e)
                {
                    System.Threading.Thread.Sleep(1000);
                    response = client.Execute(request);
                    //throw new NotImplementedException(e.ToString());
                }
            }

            if (maxAttempts == 0)
            {
                error = "No se pudo enviar el código.";
                return null;
            }

            if (respuesta.GetValue("success").ToString() == "0")
                return null;

            JObject json = new JObject();

            submission.CodigoToken = respuesta.GetValue("files")[0].Last().First().ToString();
            submission.Username = username;
            submission.ContestId = contestId;
            submission.ProblemName = problema;
            submission.OIAJSubmissionId =respuesta.GetValue("id").ToString();
            submission.Fecha = localSubmissionTime;
            submission.Puntaje = (Int32.Parse(GetScore(submission.OIAJSubmissionId, token, ref json))*maxScore)/100;
            submission.Json = json;

            return submission;
        }
    }
}