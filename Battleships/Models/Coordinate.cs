using System;
using System.Collections.Generic;
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

        public Coordinate RowNext()
        {
            if (this.Row == 10)
                throw new ArgumentOutOfRangeException();

            return new Coordinate(this.Column, this.Row + 1);
        }

        public Coordinate RowPrevious()
        {
            if (this.Row == 1)
                throw new ArgumentOutOfRangeException();

            return new Coordinate(this.Column, this.Row - 1);
        }

        public Coordinate ColumnNext()
        {
            if (this.Column == 10)
                throw new ArgumentOutOfRangeException();

            return new Coordinate(this.Column + 1, this.Row);
        }

        public Coordinate ColumnPrevious()
        {
            if (this.Column == 1)
                throw new ArgumentOutOfRangeException();

            return new Coordinate(this.Column - 1, this.Row);
        }

        public List<Coordinate> GetCoordinatesAround()
        {
            var result = new List<Coordinate>();

            try
            {
                result.Add(ColumnNext());
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                result.Add(ColumnPrevious());
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                result.Add(RowNext());
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                result.Add(RowPrevious());
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                result.Add(ColumnNext().RowNext());
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                result.Add(ColumnNext().RowPrevious());
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                result.Add(ColumnPrevious().RowNext());
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                result.Add(ColumnPrevious().RowPrevious());
            }
            catch (ArgumentOutOfRangeException) { }
            
            return result;
        }
    }
}