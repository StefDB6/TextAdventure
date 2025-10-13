using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRaiders_TextAdventure;

namespace Tests
{
    [TestClass]
    public class DirectionTests
    {
        // Test dat de Direction-enum exact vier waarden bevat.
        [TestMethod]
        public void Enum_Has_Four_Directions()
        {
            var values = Enum.GetValues(typeof(Direction));
            Assert.AreEqual(4, values.Length);
        }

        //Test dat de Direction-enum de verwachte namen bevat
        [TestMethod]
        public void Enum_Contains_Expected_Names()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(Direction), Direction.North));
            Assert.IsTrue(Enum.IsDefined(typeof(Direction), Direction.East));
            Assert.IsTrue(Enum.IsDefined(typeof(Direction), Direction.South));
            Assert.IsTrue(Enum.IsDefined(typeof(Direction), Direction.West));
        }
    }
}
