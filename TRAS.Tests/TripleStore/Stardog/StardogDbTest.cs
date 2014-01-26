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
        public void CreatePersonWithInterests()
        {
            var db = StardogDb.GetInstance();

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

            ThingViewModel t1 = new ThingViewModel()
            {
                Id = Utils.GetID("th"),
                Name = "Seaside"
            };

            ThingViewModel t2 = new ThingViewModel()
            {
                Id = Utils.GetID("th"),
                Name = "Work"
            };

            db.CreateOrUpdateThing(t1);
            db.CreateOrUpdateThing(t2);

            personVM2.Intersts.Add(t1);
            personVM2.Intersts.Add(t2);

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

            ItineraryViewModel itinVM0 = new ItineraryViewModel()
            {
                Id = "itin2",
                Name = "New York - Paris",
                Rating = 6,
                Budget = 10000.5,
                NrOfDays = 4,
                StartDate = new DateTime(2014, 8, 5),
                EndDate = new DateTime(2014, 8, 9),
                Creator = new PersonViewModel() { Id = "cepure" }
            };
            
            db.CreateOrUpdateItinerary(itinVM0);
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

        [TestMethod]
        public void GetItineraries()
        {
            var db = StardogDb.GetInstance();
            var allItins = db.GetItineraries(null);
            var myItins = db.GetItineraries("cepure");
        }

        [TestMethod]
        public void CreateThing()
        {
            ThingViewModel t1 = new ThingViewModel()
            {
                Id = Utils.GetID("th"),
                Name = "Mountains"
            };

            ThingViewModel t2 = new ThingViewModel()
            {
                Id = Utils.GetID("th"),
                Name = "Maths"
            };

            var db = StardogDb.GetInstance();
            db.CreateOrUpdateThing(t1);
            db.CreateOrUpdateThing(t2);
        }

        [TestMethod]
        public void CreatePersonWithFollowing()
        {
            PersonViewModel personVM = new PersonViewModel()
            {
                Id = "cepure",
                NickName = "cepure",
                Email = "claudiu.epure@gmail.com"
            };

            personVM.Following.Add(new PersonViewModel(){Id = "atiron"});

            var db = StardogDb.GetInstance();
            db.CreateOrUpdatePerson(personVM);
        }

        [TestMethod]
        public void CreatePlace()
        {
            PlaceViewModel place = new PlaceViewModel();
        }
    }
}
