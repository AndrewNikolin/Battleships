using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Battleships.Models
{
    public class Ship
    {
        [Range(1, 4, ErrorMessage = "Size of the ship is invalid")]
        public int Size { get; set; }

        public List<Coordinate> Coordinates { get; set; }

        public bool IsValid()
        {
            if (Coordinates.Count != Size)
                return false;

            var firstCoordinate = Coordinates.FirstOrDefault();

            if (!Coordinates.All(c => c.Row == firstCoordinate.Row || c.Column == firstCoordinate.Column))
                return false;

            return true;
        }
    }
}
