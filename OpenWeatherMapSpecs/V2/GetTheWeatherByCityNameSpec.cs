using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBDD.V2;
using OpenWeatherMapSpecs.V2.Specs;

namespace OpenWeatherMapSpecs.V2
{
    [TestClass]
    public class GetTheWeatherByCityNameSpec
    {
        [TestMethod]
        public void Demo()
        {
            TheWeatherByCityNameSpec.Execute(@"London", @"uk");
        }
    }
}