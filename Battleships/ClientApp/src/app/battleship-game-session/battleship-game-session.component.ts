import { Component, OnInit, OnDestroy } from '@angular/core';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
import { Router, ActivatedRoute } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { HubConnection } from '@aspnet/signalr';
import { Battlefield } from '../models/battlefield';
import { Coordinate } from '../models/coordinate';
import { CellState } from '../models/cell-state';

const gameHub = '/BattleshipGameSession';
const playerCookie = 'CaptainName';
const playerIdCookie = 'CaptainId';
const hubRegisterPlayerInGame = 'RegisterPlayer';
const TryPlaceShip = 'PlaceShip';
const FireAtCell = 'FireAtCell';

@Component({
  selector: 'app-battleship-game-session',
  templateUrl: './battleship-game-session.component.html',
  styleUrls: ['./battleship-game-session.component.css']
})
export class BattleshipGameSessionComponent implements OnInit, OnDestroy {
  private gameHub: HubConnection;
  private shipsList: number[] = [1, 1, 1, 1, 2, 2, 2, 3, 3, 4];
  private currentShip: number = -1;
  private currentShipCoordinates: Coordinate[];
  private gameId: string;
  private allowFire: boolean = false;
  playerBattlefield: Battlefield;
  opponentBattlefield: Battlefield;
  public opponentName: string = '';
  infoMessage = "Initializing connection...";

  ngOnDestroy(): void {
    //throw new Error("Method not implemented.");
  }

  constructor(private cookieService: CookieService,
    private router: Router,
    private spinnerService: Ng4LoadingSpinnerService,
    private route: ActivatedRoute) {
    this.playerBattlefield = new Battlefield();
    this.opponentBattlefield = new Battlefield();
  }

  async ngOnInit() {
    this.spinnerService.show();

    this.gameHub = new HubConnection(gameHub);

    this.gameHub.on('startPlacingShips', (opponentName) => this.startPlacingShips(opponentName));
    this.gameHub.on('nextTurn', (turn) => this.nextTurn(turn));
    this.gameHub.on('gameWon', () => this.gameFinished(true));
    this.gameHub.on('gameLost', () => this.gameFinished(false));
    this.gameHub.on('updateCellState', (cellState: CellState, cellCoordinate: Coordinate) => this.updateCellState(cellCoordinate, cellState));

    await this.gameHub.start();

    this.infoMessage = 'Connecting to game server...';

    this.registerPlayer();

    this.infoMessage = 'Waiting for second player to connect...';

    //this.spinnerService.hide();
  }

  async registerPlayer() {
    var playerId = this.cookieService.get(playerIdCookie);
    this.gameId = this.route.snapshot.paramMap.get('gameId');

    await this.gameHub.invoke(hubRegisterPlayerInGame, playerId, this.gameId);
  }

  gameFinished(youWon: boolean) {
    if (youWon)
      this.infoMessage = 'Congratulations, you won!';
    else
      this.infoMessage = `Unfortunately, ${this.opponentName} won`;
  }

  async ownCellClick(cell: Coordinate) {
    if (this.currentShip === -1 || !this.currentShip) {
      return;
    }

    this.currentShipCoordinates.push(cell);
    cell.state = CellState.PreShip;

    if (this.currentShipCoordinates.length === this.currentShip) {
      await this.gameHub.invoke(TryPlaceShip, this.currentShipCoordinates, this.gameId).then(succeeded => {
        if (!succeeded) {
          this.currentShipCoordinates.forEach(cell => cell.state = CellState.Empty);
          this.currentShipCoordinates = [];
          this.infoMessage = `You can't place ship in this manner! Place ship with size of ${this.currentShip}`;
        } else {
          this.currentShipCoordinates.forEach(cell => cell.state = CellState.Ship);
          this.currentShip = -1;
          this.placeNextShip();
        }
      });
    }
  }

  async opponentCellClick(cell: Coordinate) {
    if (!this.allowFire)
      return;

    await this.gameHub.invoke(FireAtCell, cell, this.gameId).then((result: CellState) => {
      cell.state = result;
    });
  }

  startPlacingShips(opponentName: string) {
    this.spinnerService.hide();

    this.opponentName = opponentName;

    this.placeNextShip();
  }

  nextTurn(turn: boolean) {
    this.allowFire = turn;
    if (this.allowFire) {
      this.infoMessage = 'Your turn';
    } else {
      this.infoMessage = `${this.opponentName}'s turn`;
    }
  }

  placeNextShip() {
    this.currentShipCoordinates = [];
    this.currentShip = this.shipsList.pop();

    if (this.currentShip) {
      this.infoMessage = `Place ship with size of ${this.currentShip}`;
    } else {
      this.infoMessage = 'Waiting for the game to start...';
    }
  }

  updateCellState(cellCoordinate: Coordinate, cellState: CellState): void {
    this.playerBattlefield.updateCellState(cellCoordinate, cellState);
  }
}
