import { CellState } from './cell-state';

export class Coordinate {
  column: number;
  row: number;
  state: CellState;

  constructor(values: Object = {}) {
    (<any>Object).assign(this, values);
  }
}
