export class Player {
  playerId: string;
  playerName: string;

  constructor(values: Object = {}) {
    (<any>Object).assign(this, values);
  }

}
