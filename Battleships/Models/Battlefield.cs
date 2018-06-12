using System;
using System.Collections.Generic;
using System.Linq;

namespace Battleships.Models
{
    public class Battlefield
    {
        private readonly Dictionary<Coordinate, CellState> _field = new Dictionary<Coordinate, CellState>();
        private readonly List<Dictionary<Coordinate, CellState>> _placedShips = new List<Dictionary<Coordinate, CellState>>();

        public bool AllShipsPlaced => _placedShips.Count == 10;

        public bool AllShipsDestroyed => _field.All(c => c.Value != CellState.Ship);

        public Battlefield()
        {
            for (var i = 1; i <= 10; i++)
            {
                for (var j = 1; j <= 10; j++)
                    _field[new Coordinate(i, j)] = CellState.Empty;
            }
        }

        public Dictionary<Coordinate, CellState> FireAtCell(Coordinate cellCoordinate)
        {
            var result = new Dictionary<Coordinate, CellState>();

            switch (_field[cellCoordinate])
            {
                case CellState.Empty:
                    _field[cellCoordinate] = CellState.Miss;
                    result.Add(cellCoordinate, CellState.Miss);
                    break;
                case CellState.Ship:
                    _field[cellCoordinate] = CellState.Damaged;
                    result.Add(cellCoordinate, CellState.Damaged);

                    var ship = _placedShips.First(s => s.ContainsKey(cellCoordinate));
                    ship[cellCoordinate] = CellState.Damaged;
                    if (ship.All(c => c.Value == CellState.Damaged))
                    {
                        foreach (var cell in ship)
                        {
                            var damagedCells = cell.Key.GetCoordinatesAround();

                            damagedCells.ForEach(c =>
                            {
                                _field[c] = CellState.Damaged;
                                result[c] = CellState.Damaged;
                            });
                        }
                    }

                    break;
                case CellState.Miss:
                    result.Add(cellCoordinate, CellState.Miss);
                    break;
                case CellState.Damaged:
                    result.Add(cellCoordinate, CellState.Damaged);
                    break;
                default:
                    result.Add(cellCoordinate, CellState.Empty);
                    break;
            }

            return result;
        }

        public bool TryPlaceShip(IEnumerable<Coordinate> coordinates)
        {
            var coordinatesList = coordinates.ToList();
            var shipLength = coordinatesList.Count;

            if (!coordinatesList.Any())
                return false;

            if (!ValidateShipSize(shipLength))
                return false;

            if (coordinatesList.Distinct().Count() != shipLength)
                return false;

            var firstCoordinate = coordinatesList.FirstOrDefault();

            if (coordinatesList.All(c => c.Column.Equals(firstCoordinate.Column)))
            {
                coordinatesList = coordinatesList.OrderBy(c => c.Row).ToList();

                firstCoordinate = coordinatesList.FirstOrDefault();

                if (coordinatesList.LastOrDefault().Row != firstCoordinate.Row + coordinatesList.Count - 1)
                    return false;
            }
            else if (coordinatesList.All(c => c.Row.Equals(firstCoordinate.Row)))
            {
                coordinatesList = coordinatesList.OrderBy(c => c.Column).ToList();

                firstCoordinate = coordinatesList.FirstOrDefault();

                if (coordinatesList.LastOrDefault().Column != firstCoordinate.Column + coordinatesList.Count - 1)
                    return false;
            }
            else
            {
                return false;
            }

            foreach (var coordinate in coordinatesList)
            {
                var result = ValidateSingleCoordinate(coordinate);
                if (!result)
                    return false;
            }

            foreach (var coordinate in coordinatesList)
            {
                _field[coordinate] = CellState.Ship;
            }

            var newShip = new Dictionary<Coordinate, CellState>();
            coordinatesList.ForEach(c => newShip.Add(c, CellState.Ship));

            _placedShips.Add(newShip);

            return true;
        }

        private bool ValidateSingleCoordinate(Coordinate coordinate)
        {
            var result = ValidateRightCell(coordinate) && ValidateLeftCell(coordinate) && ValidateTopCell(coordinate) &&
                          ValidateBottomCell(coordinate) && ValidateTopLeftCell(coordinate) &&
                          ValidateTopRightCell(coordinate) && ValidateBottomLeftCell(coordinate) &&
                          ValidateBottomRightCell(coordinate);

            return result;
        }

        private bool ValidateBottomCell(Coordinate coordinate)
        {
            try
            {
                var checkCell = coordinate.RowNext();
                return _field[checkCell] != CellState.Ship;
            }
            catch (ArgumentOutOfRangeException)
            {
                return true;
            }
        }

        private bool ValidateTopCell(Coordinate coordinate)
        {
            try
            {
                var checkCell = coordinate.RowPrevious();
                return _field[checkCell] != CellState.Ship;
            }
            catch (ArgumentOutOfRangeException)
            {
                return true;
            }
        }

        private bool ValidateLeftCell(Coordinate coordinate)
        {
            try
            {
                var checkCell = coordinate.ColumnPrevious();
                return _field[checkCell] != CellState.Ship;
            }
            catch (ArgumentOutOfRangeException)
            {
                return true;
            }
        }

        private bool ValidateRightCell(Coordinate coordinate)
        {
            try
            {
                var checkCell = coordinate.ColumnNext();
                return _field[checkCell] != CellState.Ship;
            }
            catch (ArgumentOutOfRangeException)
            {
                return true;
            }
        }

        private bool ValidateTopLeftCell(Coordinate coordinate)
        {
            try
            {
                var checkCell = coordinate.RowPrevious().ColumnPrevious();
                return _field[checkCell] != CellState.Ship;
            }
            catch (ArgumentOutOfRangeException)
            {
                return true;
            }
        }

        private bool ValidateTopRightCell(Coordinate coordinate)
        {
            try
            {
                var checkCell = coordinate.RowPrevious().ColumnNext();
                return _field[checkCell] != CellState.Ship;
            }
            catch (ArgumentOutOfRangeException)
            {
                return true;
            }
        }

        private bool ValidateBottomLeftCell(Coordinate coordinate)
        {
            try
            {
                var checkCell = coordinate.RowNext().ColumnPrevious();
                return _field[checkCell] != CellState.Ship;
            }
            catch (ArgumentOutOfRangeException)
            {
                return true;
            }
        }

        private bool ValidateBottomRightCell(Coordinate coordinate)
        {
            try
            {
                var checkCell = coordinate.RowNext().ColumnNext();
                return _field[checkCell] != CellState.Ship;
            }
            catch (ArgumentOutOfRangeException)
            {
                return true;
            }
        }

        private bool ValidateShipSize(int size)
        {
            if (size < 1 || size > 4)
                return false;

            switch (size)
            {
                case 4 when _placedShips.Count(s => s.Count() == 4) >= 1:
                    return false;
                case 3 when _placedShips.Count(s => s.Count() == 3) >= 2:
                    return false;
                case 2 when _placedShips.Count(s => s.Count() == 2) >= 3:
                    return false;
                case 1 when _placedShips.Count(s => s.Count() == 1) >= 4:
                    return false;
            }

            return true;
        }
    }
}