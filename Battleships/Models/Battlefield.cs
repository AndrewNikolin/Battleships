using System.Collections.Generic;
using System.Linq;

namespace Battleships.Models
{
    public class Battlefield
    {
        private readonly Dictionary<Coordinate, CellState> _field = new Dictionary<Coordinate, CellState>();
        private readonly List<IEnumerable<Coordinate>> _placedShips = new List<IEnumerable<Coordinate>>();

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

        public CellState FireAtCell(Coordinate cellCoordinate)
        {
            switch (_field[cellCoordinate])
            {
                case CellState.Empty:
                    _field[cellCoordinate] = CellState.Miss;
                    return CellState.Miss;
                case CellState.Ship:
                    _field[cellCoordinate] = CellState.Damaged;
                    return CellState.Damaged;
                case CellState.Miss:
                    return CellState.Miss;
                case CellState.Damaged:
                    return CellState.Damaged;
                default:
                    return CellState.Empty;
            }
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

            _placedShips.Add(coordinatesList);

            return true;
        }

        private bool ValidateSingleCoordinate(Coordinate coordinate)
        {
            bool result;

            if (coordinate.Row == 1)
            {
                result = ValidateBottomCell(coordinate);

                if (coordinate.Column == 1)
                {
                    result = result && ValidateRightCell(coordinate) && ValidateBottomRightCell(coordinate);
                }
                else if (coordinate.Column == 10)
                {
                    result = result && ValidateLeftCell(coordinate) && ValidateBottomLeftCell(coordinate);
                }
                else
                {
                    result = result && ValidateLeftCell(coordinate) && ValidateRightCell(coordinate) &&
                             ValidateBottomLeftCell(coordinate) &&
                             ValidateBottomRightCell(coordinate);
                }
            }
            else if (coordinate.Row == 10)
            {
                result = ValidateTopCell(coordinate);

                switch (coordinate.Column)
                {
                    case 1:
                        result = result && ValidateRightCell(coordinate) && ValidateTopRightCell(coordinate);
                        break;
                    case 10:
                        result = result && ValidateLeftCell(coordinate) && ValidateTopLeftCell(coordinate);
                        break;
                    default:
                        result = result && ValidateLeftCell(coordinate) && ValidateRightCell(coordinate) &&
                                 ValidateTopLeftCell(coordinate) &&
                                 ValidateTopRightCell(coordinate);
                        break;
                }
            }
            else if (coordinate.Column == 1)
            {
                result = ValidateRightCell(coordinate) && ValidateTopCell(coordinate) && ValidateBottomCell(coordinate) &&
                         ValidateBottomRightCell(coordinate) &&
                         ValidateTopRightCell(coordinate);
            }
            else if (coordinate.Column == 10)
            {

                result = ValidateLeftCell(coordinate) && ValidateTopCell(coordinate) && ValidateBottomCell(coordinate) &&
                         ValidateBottomLeftCell(coordinate) &&
                         ValidateTopLeftCell(coordinate);
            }
            else
            {
                result = ValidateRightCell(coordinate) && ValidateLeftCell(coordinate) && ValidateTopCell(coordinate) &&
                         ValidateBottomCell(coordinate) && ValidateTopLeftCell(coordinate) &&
                         ValidateTopRightCell(coordinate) && ValidateBottomLeftCell(coordinate) &&
                         ValidateBottomRightCell(coordinate);
            }

            return result;
        }

        private bool ValidateBottomCell(Coordinate coordinate)
        {
            return _field[new Coordinate(coordinate.Column, coordinate.Row + 1)] != CellState.Ship;
        }

        private bool ValidateTopCell(Coordinate coordinate)
        {
            return _field[new Coordinate(coordinate.Column, coordinate.Row - 1)] != CellState.Ship;
        }

        private bool ValidateLeftCell(Coordinate coordinate)
        {
            return _field[new Coordinate(coordinate.Column - 1, coordinate.Row)] != CellState.Ship;
        }

        private bool ValidateRightCell(Coordinate coordinate)
        {
            return _field[new Coordinate(coordinate.Column + 1, coordinate.Row)] != CellState.Ship;
        }

        private bool ValidateTopLeftCell(Coordinate coordinate)
        {
            return _field[new Coordinate(coordinate.Column - 1, coordinate.Row - 1)] != CellState.Ship;
        }

        private bool ValidateTopRightCell(Coordinate coordinate)
        {
            return _field[new Coordinate(coordinate.Column + 1, coordinate.Row - 1)] != CellState.Ship;
        }

        private bool ValidateBottomLeftCell(Coordinate coordinate)
        {
            return _field[new Coordinate(coordinate.Column - 1, coordinate.Row + 1)] != CellState.Ship;
        }

        private bool ValidateBottomRightCell(Coordinate coordinate)
        {
            return _field[new Coordinate(coordinate.Column + 1, coordinate.Row + 1)] != CellState.Ship;
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