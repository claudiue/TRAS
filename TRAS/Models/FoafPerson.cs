using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TRAS.Models.Interfaces;

namespace TRAS.Models
{
    public class FoafPerson : IUser, IPerson
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public int Age { get; set; }
        public string Ocupation { get; set; }
        public IList<string> Interests { get; set; }
        public IList<string> Following { get; set; }
        public IList<string> Folowers { get; set; }
        public IList<string> Queries { get; set; }
    }
}