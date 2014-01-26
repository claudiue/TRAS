using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TripleStore.Stardog;
using ViewModels;

namespace TRAS.Tests.TripleStore.Stardog
{
    [TestClass]
    public class StardogDbTest
    {
        [TestMethod]
        public void CreatePerson()
        {
            var db = StardogDb.GetInstance();

            PersonViewModel personVM = new PersonViewModel()
            {
                Id = "cepure",
                NickName = "cepure",
                Email = "claudiu.epure@gmail.com"
            };

            PersonViewModel personVM2 = new PersonViewModel()
            {
                Id = "atiron",
                NickName = "atiron",
                Email = "andreeea.tiron@gmail.com",
                Age = 23, 
                FirstName = "Andreea", 
                LastName = "Tiron",
                Gender = "F"
            };

            db.CreateOrUpdatePerson(personVM2);
        }

        [TestMethod]
        public void CreateItinerary()
        {
            var db = StardogDb.GetInstance();

            ItineraryViewModel itinVM = new ItineraryViewModel()
            {
                Id = "itin1",
                Name = "Iasi-Londra",
                Rating = 7,
                Budget = 2000.5,
                NrOfDays = 7,
                StartDate = new DateTime(2014, 5, 21),
                EndDate = new DateTime(2014, 5, 28),
                Creator = new PersonViewModel() { Id = "atiron" }
            };
             
            db.CreateOrUpdateItinerary(itinVM);
        }

        [TestMethod]
        public void GetPerson()
        {
            var db = StardogDb.GetInstance();
            var personVM = db.GetPerson("atiron");
        }

        [TestMethod]
        public void GetItinerary()
        {
            var db = StardogDb.GetInstance();
            var itinVM = db.GetItinerary("itin1");
        }
    }
}
