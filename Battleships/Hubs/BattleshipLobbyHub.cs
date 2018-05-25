using System;
using System.Threading.Tasks;
using Battleships.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Battleships.Hubs
{
    public class BattleshipLobbyHub : Hub
    {
        private IGameManager _gameManager;

        public BattleshipLobbyHub(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _gameManager.DisconnectPlayer(Context.ConnectionId);
            Clients.All.SendAsync("RefreshPlayers", _gameManager.GetPlayers());

            return base.OnDisconnectedAsync(exception);
        }

        public (bool RegResult, Guid Id) RegisterPlayer(string playerName, string playerId)
        {
            var result = _gameManager.RegisterOnlinePlayer(playerName, Context.ConnectionId, playerId);

            if (!result.RegistrationSuccessfull)
            {
                return (false, Guid.Empty);
            }

            Clients.All.SendAsync("RefreshPlayers", _gameManager.GetPlayers());
            return (RegResult: true, Id: result.PlayerId);
        }

        public void AddPlayerToPlayQueue()
        {
            _gameManager.AddPlayerToQueue(Context.ConnectionId);
        }

        public void SendChatMessage(string message)
        {
            Clients.All.SendAsync("SendChatMessage",
                new {author = _gameManager.GetPlayerByConnectionId(Context.ConnectionId).Name, message});
        }
    }
}