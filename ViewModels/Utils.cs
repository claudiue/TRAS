using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public static class Utils
    {
        public static string GetID(string prefix)
        {
            return string.Concat(prefix, "_", Guid.NewGuid().ToString());
        }
    }
}
