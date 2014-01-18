using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRAS.Models.Interfaces
{
    public interface IPerson
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string Sex { get; set; }
        int Age { get; set; }
        string Ocupation { get; set; }
    }
}
