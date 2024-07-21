using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewBlazorProjecct.Shared.DTOs
{
    public class Game
    {
        public int GameID { get; set; }
        public int GameCode { get; set; }
        public string GameName { get; set; }
        public bool IsPublish { get; set; }
        public bool ScoreFormat { get; set; }
        public int UserID { get; set; }
        public List<LawsDTO> LawList { get; set; }
        public bool GameStarted { get; set; }
    }
}
