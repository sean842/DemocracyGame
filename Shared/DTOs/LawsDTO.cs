using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewBlazorProjecct.Shared.DTOs
{
	public class LawsDTO
	{
        public int GameID { get; set; }
        public int LawID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int For { get; set; }
        public int Against { get; set; }
        public int Avoid { get; set; }
        public bool IsPass { get; set; }
    }
}
