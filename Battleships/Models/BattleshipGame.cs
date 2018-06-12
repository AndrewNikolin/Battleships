using System;
using System.Collections.Generic;
using System.Linq;

namespace Battleships.Models
{
    public class BattleshipGame
    {
        public Guid GameId;

        private readonly Player _player1;
        private readonly Player _player2;

        private readonly Battlefield _player1Field;
        private readonly Battlefield _player2Field;

        public bool Player1Turn;

        public bool AllowPlacingShips => !string.IsNullOrWhiteSpace(_player1.GameSessionConnectionId) &&
                                   !string.IsNullOrWhiteSpace(_player2.GameSessionConnectionId);

        public bool AllShipsPlaced => _player1Field.AllShipsPlaced && _player2Field.AllShipsPlaced;

        public (bool gameFinished, Player winner, Player looser) GameResult
        {
            get
            {
                if (_player1Field.AllShipsDestroyed)
                {
                    return (true, _player2, _player1);
                }

                if (_player2Field.AllShipsDestroyed)
                {
                    return (true, _player1, _player2);
                }

                return (false, null, null);
            }
        }

        public BattleshipGame(Player player1, Player player2)
        {
            GameId = Guid.NewGuid();

            _player1 = player1;
            _player2 = player2;

            _player1Field = new Battlefield();
            _player2Field = new Battlefield();

            Player1Turn = (new Random()).Next(100) < 50;
        }

        public bool TryRegisterPlayer(Guid playerId, string gameSessionConnectionId)
        {
            if (_player1.PlayerId.Equals(playerId))
            {
                _player1.GameSessionConnectionId = gameSessionConnectionId;
                return true;
            }

            if (_player2.PlayerId.Equals(playerId))
            {
                _player2.GameSessionConnectionId = gameSessionConnectionId;
                return true;
            }

            return false;
        }

        public bool TryPlaceShip(IEnumerable<Coordinate> coordinates, string playerConnectionId)
        {

            if (_player1.GameSessionConnectionId.Equals(playerConnectionId))
                return _player1Field.TryPlaceShip(coordinates);

            if (_player2.GameSessionConnectionId.Equals(playerConnectionId))
                return _player2Field.TryPlaceShip(coordinates);

            return false;
        }

        public (Dictionary<Coordinate, CellState> cellStates, Player cellOwner) FireAtCell(Coordinate cellCoordinate, string playerConnectionId)
        {
            var result = new Dictionary<Coordinate, CellState>()
            {
                {cellCoordinate, CellState.Empty }
            };

            if (Player1Turn && _player1.GameSessionConnectionId.Equals(playerConnectionId))
            {
                result = _player2Field.FireAtCell(cellCoordinate);

                if (result.All(c => c.Value != CellState.Damaged))
                    Player1Turn = !Player1Turn;

                return (result, _player2);
            }

            if (!Player1Turn && _player2.GameSessionConnectionId.Equals(playerConnectionId))
            {
                result = _player1Field.FireAtCell(cellCoordinate);

                if (result.All(c => c.Value != CellState.Damaged))
                    Player1Turn = !Player1Turn;

                return (result, _player1);
            }

            return (result, null);
        }

        public (Player player1, Player player2) GetPlayers()
        {
            return (_player1, _player2);
        }
    }
}