using Battleships.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BattleshipsTests.Models
{
    [TestClass]
    public class CoordinateTests
    {
        [TestMethod]
        public void ToString_Test()
        {
            var coordinates = new Coordinate(1, 1);
            Assert.AreEqual("A1", coordinates.ToString());

            coordinates = new Coordinate(5, 5);
            Assert.AreEqual("E5", coordinates.ToString());
        }

        [TestMethod]
        public void EqualityOperatorTest()
        {
            var c1 = new Coordinate(1, 1);
            var c2 = new Coordinate(1, 1);

            Assert.IsTrue(c1 == c2);

            c2 = new Coordinate(1, 2);

            Assert.IsFalse(c1 == c2);
        }

        [TestMethod]
        public void NotEqualityOperatorTest()
        {
            var c1 = new Coordinate(1, 1);
            var c2 = new Coordinate(1, 1);

            Assert.IsFalse(c1 != c2);

            c2 = new Coordinate(1, 2);

            Assert.IsTrue(c1 != c2);

            c2 = new Coordinate(2, 1);

            Assert.IsTrue(c1 != c2);

            c2 = new Coordinate(2, 2);

            Assert.IsTrue(c1 != c2);
        }
    }
}
