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
        // Test that the Direction-enum contains exact four values.
        [TestMethod]
        public void Enum_Has_Four_Directions()
        {
            var values = Enum.GetValues(typeof(Direction));
            Assert.AreEqual(4, values.Length);
        }

        // Test that the Direction enum contains the expected names
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
