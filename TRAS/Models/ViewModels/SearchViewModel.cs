using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TRAS.Models.ViewModels
{
    public class SearchViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "Search")]
        public string Query { get; set; }


        [DataType(DataType.Text)]
        [Display(Name = "Hotels")]
        public string Hotels { get; set; }


        [DataType(DataType.Text)]
        [Display(Name = "Flights")]
        public string Flights { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Restaurants")]
        public string Restaurants { get; set; }

        public bool isChecked { get; set; }
    }
}