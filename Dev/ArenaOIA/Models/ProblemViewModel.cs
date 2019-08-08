using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArenaOIA.Models
{
    public class ProblemViewModel
    {
        public string Nombre { get; set; }
        public List<Tuple<string, string> > Archivos { get; set; }
        public string LimiteMemoria { get; set; }
        public string LimiteTiempo { get; set; }
        public string Codigo { get; set; }
        public int MaxScore { get; set; }
        public List<Submission> Envios { get; set; }
    }
}