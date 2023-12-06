using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace object_automatisch_aanmaken
{
    public class Gebruiker
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Age { get; set; }
        public string Username { get; set; }
        public string Pasword { get; set; }

        public Gebruiker(string voornaam, string achternaam, string leeftijd, string paswoord, string username)
        {
            FirstName = voornaam;
            LastName = achternaam;
            Age = leeftijd;
            Username = username;
            Pasword = paswoord;
        }
    }
}
