using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewBlazorProjecct.Shared.DTOs {
    public class GameLawCount {

        public int GameID { get; set; }
        public int GameCode { get; set; }
        public string GameName { get; set; }
        public bool IsPublish { get; set; }
        public bool ScoreFormat { get; set; }
        public int UserID { get; set; }
        public int LawsNumber { get; set; }
        public bool CanPublish { get; set; }

    }
}
