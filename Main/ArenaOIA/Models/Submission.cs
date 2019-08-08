using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArenaOIA.Models
{
    public class Submission
    {
        public string Username { get; set; }
        public string ContestId { get; set; }
        public string ProblemName { get; set; }
        public string OIAJSubmissionId { get; set; }
        public DateTime Fecha { get; set; }
        public int Puntaje { get; set; }
        public string Json { get; set; }
    }
}