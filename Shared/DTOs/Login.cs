using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace NewBlazorProjecct.Shared.DTOs {
    public class Login {

        [Required(ErrorMessage = "חובה להזין שם משתמש")]
        public string Name { get; set; }

        [Required(ErrorMessage = "חובה להזין סיסמה")]
        public int Password { get; set; }
    }
}
