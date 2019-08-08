using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArenaOIA.Models
{
    public class Contest
    {
        public string Id { get; set; }
        public string Descripcion { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
        public List<Problem> Problemas { get; set; }
        public List<string> Participantes { get; set; }
        public bool Ranking { get; set; }
    }
}