using System;

namespace Battleships.Models
{
    public class Player
    {
        public string LobbyConnectionId { get; set; }
        public string GameSessionConnectionId { get; set; }
        public string Name { get; set; }
        public Guid PlayerId { get; set; }

        public Player(string name, string lobbyConnectionId, Guid playerId)
        {
            PlayerId = playerId;
            LobbyConnectionId = lobbyConnectionId;
            Name = name;
        }

        public Player(string name, string lobbyConnectionId)
        {
            Name = name;
            LobbyConnectionId = lobbyConnectionId;
            PlayerId = Guid.NewGuid();
        }
    }
}