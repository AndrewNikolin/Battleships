import { Coordinate } from "./coordinate";
import {CellState} from "./cell-state"

export class Battlefield {
  cells: (Coordinate[])[];

  constructor() {
    this.cells = new Array<Coordinate[]>(10);
    for (let i = 1; i <= 10; i++) {
      let row = new Array(10);
      for (let j = 1; j <= 10; j++) {
        row[j-1] = new Coordinate({ row: i, column: j, state: CellState.Empty });
      }
      this.cells[i-1] = row;
    }
  }

  updateCellState(cellCoordinate: Coordinate, cellState: CellState) {
    this.cells[cellCoordinate.row - 1][cellCoordinate.column - 1].state = cellState;
  }
}
