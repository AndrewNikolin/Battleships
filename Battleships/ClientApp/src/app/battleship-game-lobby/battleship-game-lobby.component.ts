import {Component, OnInit, OnDestroy} from '@angular/core';
import {HubConnection, LogLevel, HubConnectionBuilder} from '@aspnet/signalr';
import {CookieService} from 'ngx-cookie-service';
import {Router} from "@angular/router";
import {Ng4LoadingSpinnerService} from 'ng4-loading-spinner';
import {ChatMessage} from '../models/chat-message';

const lobbyHub = '/BattleshipGameLobby';
const playerCookie = 'CaptainName';
const playerIdCookie = 'CaptainId';
const hubRegisterPlayer = 'RegisterPlayer';
const hubAddToPlayQueue = 'AddPlayerToPlayQueue';
const hubSendChatMessage = 'SendChatMessage';

const clientRefreshPlayers = 'RefreshPlayers';
const clientGameStarted = 'GameStarted';

@Component({
  selector: 'app-battleship-game-lobby',
  templateUrl: './battleship-game-lobby.component.html',
  styleUrls: ['./battleship-game-lobby.component.css']
})
export class BattleshipGameLobbyComponent implements OnInit, OnDestroy {
  private gameLobbyHub: HubConnection;
  private playerName: string;
  private playerId: string;
  private listOfPlayers: string[];
  private chatMessages: ChatMessage[] = [];
  public infoMessage = '';

  constructor(private cookieService: CookieService, private router: Router,
              private spinnerService: Ng4LoadingSpinnerService) {
  }

  async ngOnInit() {
    this.infoMessage = 'Connecting to game server...';
    this.spinnerService.show();

    this.gameLobbyHub = new HubConnectionBuilder()
      .withUrl(lobbyHub)
      .configureLogging(LogLevel.Information)
      .build();

    this.gameLobbyHub.on(clientRefreshPlayers, (playersList) => this.refreshPlayersList(playersList));
    this.gameLobbyHub.on(hubSendChatMessage, (message) => this.updateChat(message));
    this.gameLobbyHub.on(clientGameStarted, (gameId) => this.gameStarted(gameId));

    await this.gameLobbyHub.start();

    if (this.cookieService.check(playerCookie)) {
      let cookiePlayerName = this.cookieService.get(playerCookie);
      let cookiePlayerId = this.cookieService.get(playerIdCookie);

      this.gameLobbyHub.invoke(hubRegisterPlayer, cookiePlayerName, cookiePlayerId).then(result => {
        this.processRegistration(result.item1, cookiePlayerName, cookiePlayerId);
      });
    }
    else
    {
      this.spinnerService.hide();
    }
  }

  async ngOnDestroy() {
    await this.gameLobbyHub.stop();
  }

  gameInitialized(): boolean {
    return !!this.playerName;
  }

  setUp(tempPlayerName: string) {
    this.infoMessage = 'Registering in game...';
    this.spinnerService.show();

    this.gameLobbyHub.invoke(hubRegisterPlayer, tempPlayerName, "").then(result => {
      this.processRegistration(result.item1, tempPlayerName, result.item2);
    });
  }

  addToPlayQueue() {
    this.infoMessage = 'Waiting for another player...';
    this.spinnerService.show();

    this.gameLobbyHub.invoke(hubAddToPlayQueue);
  }

  refreshPlayersList(playersList: string[]) {
    this.listOfPlayers = playersList;
  }

  sendMessage(message: string) {
    if (message) {
      this.gameLobbyHub.invoke(hubSendChatMessage, message);
    }
  }

  updateChat(message: ChatMessage) {
    this.chatMessages.push(message);
  }

  processRegistration(registrationSucceeded: boolean, playerName: string, playerId: string) {
    if (registrationSucceeded) {
      this.playerName = playerName;
      this.playerId = playerId;
      this.cookieService.set(playerCookie, this.playerName, 366);
      this.cookieService.set(playerIdCookie, this.playerId, 366);

      let systemMessage = new ChatMessage({ author: "System", message: `Welcome to the battle, ${this.playerName}`});

      this.chatMessages.push(systemMessage);
      this.spinnerService.hide();
    } else {
      this.spinnerService.hide();
      alert(`Looks like this name is already taken`);
    }
  }

  gameStarted(gameId: string) {
    this.spinnerService.hide();
    this.router.navigate(['battleship-game', gameId]);
  }
}
