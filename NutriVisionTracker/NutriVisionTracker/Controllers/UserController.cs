using NutriVisionTracker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriVisionTracker.Controllers
{
    public static class UserController
    {
        static User? User { get; set; }

        public static void CreateNewUser(string name, string email, string password) 
        {
            
        }

        public static bool SignIn() 
        {
            return false;
        }

        public static void SignOut() 
        {
            User = null;
        }
        
        private static bool CheckPassword(string pwdInput) 
        {
            return false;
        }

    }
}
