using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ItineraryViewModel
    {
        public ItineraryViewModel()
        {
            this.Features = new List<FeatureViewModel>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public int Rating { get; set; }
        public double Budget { get; set; }
        public int NrOfDays { get; set; }
        public PersonViewModel Creator { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IList<FeatureViewModel> Features { get; set; }
    }
}
