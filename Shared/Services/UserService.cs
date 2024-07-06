using NewBlazorProjecct.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewBlazorProjecct.Shared.Services {
    public class UserService {
        public bool IsLoggedIn { get; private set; }
        public User LoggedInUser { get; private set; }

        public void Login(User user) {
            IsLoggedIn = true;
            LoggedInUser = user;
        }

        public void Logout() {
            IsLoggedIn = false;
            LoggedInUser = null;
        }
    }
}
