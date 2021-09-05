using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCADAcore.Model
{
    public class User
    {
        [Key]
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }

        public User() { }

        public User(string username, string password, UserRole role) 
        {
            Username = username;
            Password = password;
            Role = role;
        }

        public override string ToString()
        {
            return $"Korisnik: {Username} sa ulogom: {Role}";
        }
    }
}