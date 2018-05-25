using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Battleships.Interfaces;
using Battleships.Models;
using Microsoft.AspNetCore.SignalR;

namespace Battleships.Hubs
{
    public class BattleshipGameHub : Hub
    {
        private IGameManager _gameManager;

        public BattleshipGameHub(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void SendChatMessage(string message)
        {
            Clients.All.SendAsync("SendChatMessage",
                new { author = _gameManager.GetPlayerByConnectionId(Context.ConnectionId).Name, message });
        }

        public bool RegisterPlayer(string playerId, string gameId)
        {
            return _gameManager.RegisterPlayerInGame(playerId, gameId, Context.ConnectionId);
        }

        public bool PlaceShip(IEnumerable<Coordinate> shipCoordinates, string gameId)
        {
            return _gameManager.TryPlaceShip(shipCoordinates, gameId, Context.ConnectionId);
        }

        public async Task<CellState> FireAtCell(Coordinate cellCoordinate, string gameId)
        {
            return await _gameManager.FireAtCell(cellCoordinate, gameId, Context.ConnectionId);
        }
    }
}
