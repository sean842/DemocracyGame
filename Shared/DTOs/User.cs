﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewBlazorProjecct.Shared.DTOs {
    public class User {

        public int UserID { get; set; }
        public string Name { get; set; }
        public List<GameLawCount> GameList { get; set; }
    }
}
