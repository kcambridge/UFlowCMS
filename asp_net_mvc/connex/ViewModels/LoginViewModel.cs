using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace connex.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password {get;set;}
        [Required]
        public bool Remember_Me { get; set; }

        public LoginViewModel()
        {
            this.Username = "";
            this.Password = "";
            this.Remember_Me = false;
        }

        public LoginViewModel(string Username, string Password, bool Remember_Me)
        {
            this.Username = Username;
            this.Password = Password;
            this.Remember_Me = Remember_Me;
        }
    }
}