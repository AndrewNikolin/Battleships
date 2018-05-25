using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Battleships.Models;

namespace Battleships.Interfaces
{
    public interface IGameManager
    {
        void AddPlayerToQueue(string connectionId);

        (bool RegistrationSuccessfull, Guid PlayerId) RegisterOnlinePlayer(string name, string connectionId, string playerId);

        void DisconnectPlayer(string connectionId);

        IEnumerable<string> GetPlayers();

        Player GetPlayerByConnectionId(string connectionId);

        bool RegisterPlayerInGame(string playerId, string gameId, string gameSessionConnectionId);

        bool TryPlaceShip(IEnumerable<Coordinate> shipCoordinates, string playerConnectionId, string gameId);

        Task<CellState> FireAtCell(Coordinate cellCoordinate, string gameId, string contextConnectionId);
    }
}