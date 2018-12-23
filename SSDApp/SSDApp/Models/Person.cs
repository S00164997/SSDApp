using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft;

namespace SSDApp.Models
{
   public class Person
    {
        [Newtonsoft.Json.JsonProperty("id")]
        public string Id { get; set; }

        [Newtonsoft.Json.JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [Newtonsoft.Json.JsonProperty("LastName")]
        public string LastName { get; set; }

        [Newtonsoft.Json.JsonProperty("Age")]
        public string Age { get; set; }

        [Newtonsoft.Json.JsonProperty("PPSN")]
        public string PPSN { get; set; }

        [Newtonsoft.Json.JsonProperty("CardNumber")]
        public string CardNumber { get; set; }

       

        public Person()
        {
        }

        public Person(string id, string firstName, string lastName, string age, string ppsn, string cardNumber)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Age = age;
            this.PPSN = ppsn;
            this.CardNumber = cardNumber;
        }
    }
}
