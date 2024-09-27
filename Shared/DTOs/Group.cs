//זה הפרויקט של שון

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewBlazorProjecct.Shared.DTOs
{
    public class Group
    {
        public int GroupID { get; set; }
        public string? ConnectionID { get; set; } = "";
        public string? GroupName { get; set; } = "";
        public int GameID { get; set; }
        public int Points { get; set; }
        public string Character { get; set; } = "";
        public string PassportCharacter { get; set; } = "";
    }
}
