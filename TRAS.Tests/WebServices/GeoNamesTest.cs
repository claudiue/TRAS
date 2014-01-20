using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebServices;
using System.Diagnostics;

namespace TRAS.Tests.WebServices
{
    [TestClass]
    public class GeoNamesTest
    {
        [TestMethod]
        public void Search()
        {
            var agent = new GeoNamesAgent();
            var toponym = agent.Search("Paris", "rdf");

            agent.FindNearBy(toponym);
        }

        [TestMethod]
        public void GetSpots()
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();
            var agent = new GeoNamesAgent();
            var res = agent.SearchSpots("Paris");
            sw.Stop();
            var time = sw.Elapsed;
        }
    }
}
