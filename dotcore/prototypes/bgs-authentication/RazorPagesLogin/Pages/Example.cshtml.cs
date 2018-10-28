using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Windows.Forms;
using TestDB.Models;
using Gorba.Common.Utility.Core;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Manage.Internal;


namespace RazorPagesLogin.Pages
{
    public class ExampleModel : PageModel
    {
        public void OnGet()
        {

        }

        public void OnPost()
        {
            string GetuserName = Request.Form["username"];
            string Getpassword = Request.Form["password"];

            string hashedPassword = SecurityUtility.Md5(Getpassword);

            bool isFlag=ExampleModelUserAuthentication(GetuserName, hashedPassword);
            if (isFlag == true)
            {


            }
            else
            {
                
                
            }

        }

        public bool ExampleModelUserAuthentication(string strUserName, string hashedPassword)
        {
            GorbaCenterBackgroundSystemContext context=new GorbaCenterBackgroundSystemContext();
            //_context = context;

            if (context.Users.Count() == 0)
            {
                // Create a new TodoItem if collection is empty,
                // which means you can't delete all TodoItems.
                context.Users.Add(new Users { Username = "admin" });
                context.SaveChanges();
            }

            //var userAdmin = "admin";
            //var passwordadmin = "1adbb3178591fd5bb0c248518f39bf6d";

            var userName = context.Users.FirstOrDefault(a => a.Username == strUserName && a.HashedPassword == hashedPassword);
           
            if (userName!=null)
            {
                return true;
                
            }
            //bool verifyUsername = String.Equals(GetuserName, userName.Username);
            //bool verifyPassword = String.Equals(hashedPassword, passWord);

            //var text = "admin";
            //var hashedPassword = SecurityUtility.Md5(text);

            //public override bool Equals(object obj)
            //{
            //    var other = obj as UserCredentials;
            //    return other != null && string.Equals(this.Username, other.Username)
            //                         && string.Equals(this.HashedPassword, other.HashedPassword);
            //}
            return false;
        }

        [BindProperty]
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}