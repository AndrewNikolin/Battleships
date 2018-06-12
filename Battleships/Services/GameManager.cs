using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Battleships.Hubs;
using Battleships.Interfaces;
using Battleships.Models;
using Microsoft.AspNetCore.SignalR;

namespace Battleships.Services
{
    public class GameManager : IGameManager
    {
        private readonly List<BattleshipGame> _liveGames;
        private readonly Queue<Player> _waitingPlayers;
        private readonly List<Player> _livePlayers;
        private IHubContext<BattleshipLobbyHub> gameLobbyHub;
        private IHubContext<BattleshipGameHub> gameSessionHub;

        public GameManager(IHubContext<BattleshipLobbyHub> lobbyHubContext, IHubContext<BattleshipGameHub> gameSessionHubContext)
        {
            _liveGames = new List<BattleshipGame>();
            _waitingPlayers = new Queue<Player>();
            _livePlayers = new List<Player>();
            gameLobbyHub = lobbyHubContext;
            gameSessionHub = gameSessionHubContext;
        }

        public void AddPlayerToQueue(string connectionId)
        {
            var playingPlayer = _livePlayers.FirstOrDefault(p => p.LobbyConnectionId.Equals(connectionId));

            if (playingPlayer == null) return;

            if (_waitingPlayers.Count > 0)
            {
                var player1 = _waitingPlayers.Dequeue();
                var newGame = new BattleshipGame(player1, playingPlayer);

                _liveGames.Add(newGame);
                gameLobbyHub.Clients.Client(playingPlayer.LobbyConnectionId).SendAsync("GameStarted", newGame.GameId);
                gameLobbyHub.Clients.Client(player1.LobbyConnectionId).SendAsync("GameStarted", newGame.GameId);
            }
            else
            {
                if (_waitingPlayers.Any(p => p == playingPlayer))
                    return;
                _waitingPlayers.Enqueue(playingPlayer);
            }
        }

        public (bool RegistrationSuccessfull, Guid PlayerId) RegisterOnlinePlayer(string name, string connectionId, string playerId)
        {
            Player player;
            if (Guid.TryParse(playerId, out var playerGuid))
            {
                if (_livePlayers.Any(p => p.PlayerId.Equals(playerGuid)))
                {
                    return (false, Guid.Empty);
                }
                else
                {
                    player = new Player(name, connectionId, playerGuid);
                    _livePlayers.Add(player);
                    return (true, player.PlayerId);
                }
            }

            player = new Player(name, connectionId);

            _livePlayers.Add(player);
            return (true, player.PlayerId);
        }

        public void DisconnectPlayer(string connectionId)
        {
            var playerToRemove = _livePlayers.FirstOrDefault(p => p.LobbyConnectionId.Equals(connectionId));
            _livePlayers.Remove(playerToRemove);
        }

        public IEnumerable<string> GetPlayers()
        {
            return _livePlayers.Select(p => p.Name);
        }

        public Player GetPlayerByConnectionId(string connectionId)
        {
            return _livePlayers.FirstOrDefault(p => p.LobbyConnectionId.Equals(connectionId));
        }

        public bool RegisterPlayerInGame(string playerId, string gameId, string gameSessionConnectionId)
        {
            if (Guid.TryParse(gameId, out var gameGuid))
            {
                if (!_liveGames.Any(g => g.GameId.Equals(gameGuid)))
                {
                    return false;
                }

                if (Guid.TryParse(playerId, out var playerGuid))
                {
                    var game = _liveGames.Single(g => g.GameId.Equals(gameGuid));
                    if (game.TryRegisterPlayer(playerGuid, gameSessionConnectionId))
                    {
                        if (game.AllowPlacingShips)
                        {
                            var players = game.GetPlayers();
                            gameSessionHub.Clients.Client(players.player1.GameSessionConnectionId).SendAsync("startPlacingShips", players.player2.Name);
                            gameSessionHub.Clients.Client(players.player2.GameSessionConnectionId).SendAsync("startPlacingShips", players.player1.Name);
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        public bool TryPlaceShip(IEnumerable<Coordinate> shipCoordinates, string gameId, string playerConnectionId)
        {
            if (Guid.TryParse(gameId, out var gameGuid))
            {
                if (!_liveGames.Any(g => g.GameId.Equals(gameGuid)))
                {
                    return false;
                }


                var game = _liveGames.Single(g => g.GameId.Equals(gameGuid));

                if (game.TryPlaceShip(shipCoordinates, playerConnectionId))
                {
                    if (game.AllShipsPlaced)
                    {
                        NextTurn(game);
                    }

                    return true;
                }
            }

            return false;
        }

        public async Task<CellState> FireAtCell(Coordinate cellCoordinate, string gameId, string contextConnectionId)
        {
            if (Guid.TryParse(gameId, out var gameGuid))
            {
                if (!_liveGames.Any(g => g.GameId.Equals(gameGuid)))
                {
                    return CellState.Empty;
                }
                
                var game = _liveGames.Single(g => g.GameId.Equals(gameGuid));

                var result = game.FireAtCell(cellCoordinate, contextConnectionId);

                foreach (var cellState in result.cellStates)
                {
                    await gameSessionHub.Clients.Client(result.cellOwner.GameSessionConnectionId)
                        .SendAsync("updateCellState", cellState.Value, cellState.Key);

                    await gameSessionHub.Clients.Client(contextConnectionId)
                        .SendAsync("updateCellState", cellState.Value, cellState.Key, false);
                }

                NextTurn(game);

                return result.cellStates.Single(c => c.Key == cellCoordinate).Value;
            }

            return CellState.Empty;
        }

        private void NextTurn(BattleshipGame game)
        {
            if (game.GameResult.gameFinished)
            {
                gameSessionHub.Clients.Client(game.GameResult.winner.GameSessionConnectionId).SendAsync("gameWon");
                gameSessionHub.Clients.Client(game.GameResult.looser.GameSessionConnectionId).SendAsync("gameLost");

                return;
            }

            var players = game.GetPlayers();

            gameSessionHub.Clients.Client(players.player1.GameSessionConnectionId).SendAsync("nextTurn", game.Player1Turn);
            gameSessionHub.Clients.Client(players.player2.GameSessionConnectionId).SendAsync("nextTurn", !game.Player1Turn);
        }
    }
}