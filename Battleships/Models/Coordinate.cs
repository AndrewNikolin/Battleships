using System;
using System.ComponentModel.DataAnnotations;

namespace Battleships.Models
{
    public struct Coordinate
    {
        [Range(1, 10)]
        public int Row { get; set; }

        [Range(1, 10)]
        public int Column { get; set; }

        public override string ToString()
        {
            char a = 'A';
            char column = (char)((int)a + Column - 1);
            return $"{column}{Row}";
        }

        public Coordinate(int column, int row)
        {
            if (column < 1 || column > 10)
                throw new ArgumentOutOfRangeException(nameof(column));

            if (row < 1 || row > 10)
                throw new ArgumentOutOfRangeException(nameof(row));

            Column = column;
            Row = row;
        }

        public static bool operator ==(Coordinate c1, Coordinate c2)
        {
            return c1.Row.Equals(c2.Row) && c1.Column.Equals(c2.Column);
        }

        public static bool operator !=(Coordinate c1, Coordinate c2)
        {
            return !c1.Row.Equals(c2.Row) || !c1.Column.Equals(c2.Column);
        }
    }
}