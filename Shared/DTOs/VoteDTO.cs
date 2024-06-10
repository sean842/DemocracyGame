using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewBlazorProjecct.Shared.DTOs {
    public class VoteDTO {
        public int GameID { get; set; }
        public int LawID { get; set; }
        public string VoteType { get; set; } // "For", "Against", or "Avoid"
    }
}
