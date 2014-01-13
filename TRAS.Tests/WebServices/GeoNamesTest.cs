using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebServices;

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
    }
}
