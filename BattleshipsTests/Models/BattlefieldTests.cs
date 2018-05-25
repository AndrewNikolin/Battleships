using System.Collections.Generic;
using Battleships.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BattleshipsTests.Models
{
    [TestClass]
    public class BattlefieldTests
    {
        [TestMethod]
        public void TryPlaceShip_Test()
        {
            var battlefield = new Battlefield();

            var badShip = new List<Coordinate>();
            var goodShip = new List<Coordinate>();

            Assert.IsFalse(battlefield.TryPlaceShip(badShip));

            badShip.Add(new Coordinate(1, 1));
            badShip.Add(new Coordinate(2, 2));
            badShip.Add(new Coordinate(3, 3));

            Assert.IsFalse(battlefield.TryPlaceShip(badShip));

            badShip.Clear();
            badShip.Add(new Coordinate(1, 1));
            badShip.Add(new Coordinate(1, 2));
            badShip.Add(new Coordinate(1, 4));

            Assert.IsFalse(battlefield.TryPlaceShip(badShip));

            badShip.Clear();
            badShip.Add(new Coordinate(1, 1));
            badShip.Add(new Coordinate(2, 1));
            badShip.Add(new Coordinate(4, 1));

            Assert.IsFalse(battlefield.TryPlaceShip(badShip));


            goodShip.Add(new Coordinate(1, 1));
            goodShip.Add(new Coordinate(1, 2));
            goodShip.Add(new Coordinate(1, 3));

            Assert.IsTrue(battlefield.TryPlaceShip(goodShip));

            goodShip.Clear();
            goodShip.Add(new Coordinate(3, 1));
            goodShip.Add(new Coordinate(4, 1));
            goodShip.Add(new Coordinate(5, 1));

            Assert.IsTrue(battlefield.TryPlaceShip(goodShip));

            goodShip.Clear();
            goodShip.Add(new Coordinate(6,6));

            Assert.IsTrue(battlefield.TryPlaceShip(goodShip));
        }

        [TestMethod]
        public void PlaceAllShips_Correct_Test()
        {
            var shipsList = new List<List<Coordinate>>();

            var s4 = new List<Coordinate>
            {
                new Coordinate(1,1),
                new Coordinate(1,2),
                new Coordinate(1,3),
                new Coordinate(1,4)
            };

            shipsList.Add(s4);

            var s3_1 = new List<Coordinate>
            {
                new Coordinate(8,1),
                new Coordinate(9,1),
                new Coordinate(10,1)
            };

            shipsList.Add(s3_1);

            var s3_2 = new List<Coordinate>
            {
                new Coordinate(10,3),
                new Coordinate(10,4),
                new Coordinate(10,5)
            };

            shipsList.Add(s3_2);

            var s2_1 = new List<Coordinate>
            {
                new Coordinate(9,10),
                new Coordinate(10,10)
            };

            shipsList.Add(s2_1);

            var s2_2 = new List<Coordinate>
            {
                new Coordinate(5,10),
                new Coordinate(6,10)
            };

            shipsList.Add(s2_2);

            var s2_3 = new List<Coordinate>
            {
                new Coordinate(1,10),
                new Coordinate(2,10)
            };

            shipsList.Add(s2_3);

            var s1_1 = new List<Coordinate>
            {
                new Coordinate(1,7)
            };

            shipsList.Add(s1_1);

            var s1_2 = new List<Coordinate>
            {
                new Coordinate(4,1)
            };

            shipsList.Add(s1_2);

            var s1_3 = new List<Coordinate>
            {
                new Coordinate(5,6)
            };

            shipsList.Add(s1_3);

            var s1_4 = new List<Coordinate>
            {
                new Coordinate(8,8)
            };

            shipsList.Add(s1_4);

            var battlefield = new Battlefield();

            foreach (var ship in shipsList)
            {
                Assert.IsTrue(battlefield.TryPlaceShip(ship));
            }
        }

        [TestMethod]
        public void PlaceAllShips_InCorrect_Test()
        {
            var shipsList = new List<List<Coordinate>>();

            var s4 = new List<Coordinate>
            {
                new Coordinate(1,1),
                new Coordinate(1,2),
                new Coordinate(1,3),
                new Coordinate(1,4)
            };

            shipsList.Add(s4);

            var s3_1 = new List<Coordinate>
            {
                new Coordinate(8,1),
                new Coordinate(9,1),
                new Coordinate(10,1)
            };

            shipsList.Add(s3_1);

            var s3_2 = new List<Coordinate>
            {
                new Coordinate(10,3),
                new Coordinate(10,4),
                new Coordinate(10,5)
            };

            shipsList.Add(s3_2);

            var s2_1 = new List<Coordinate>
            {
                new Coordinate(9,10),
                new Coordinate(10,10)
            };

            shipsList.Add(s2_1);

            var s2_2 = new List<Coordinate>
            {
                new Coordinate(5,10),
                new Coordinate(6,10)
            };

            shipsList.Add(s2_2);

            var s2_3 = new List<Coordinate>
            {
                new Coordinate(1,10),
                new Coordinate(2,10)
            };

            shipsList.Add(s2_3);

            var s1_1 = new List<Coordinate>
            {
                new Coordinate(1,7)
            };

            shipsList.Add(s1_1);

            var s1_2 = new List<Coordinate>
            {
                new Coordinate(4,1)
            };

            shipsList.Add(s1_2);

            var s1_3 = new List<Coordinate>
            {
                new Coordinate(5,6)
            };

            shipsList.Add(s1_3);

            var s1_4 = new List<Coordinate>
            {
                new Coordinate(5,7)
            };

            shipsList.Add(s1_4);

            var battlefield = new Battlefield();
            var result = true;

            foreach (var ship in shipsList)
            {
                result = result && battlefield.TryPlaceShip(ship);
            }

            Assert.IsFalse(result);
        }
    }
}
