export type CellEnum = 0 | 1 | 2;

export type GameStatusWire = number | 'InProgress' | 'XWon' | 'OWon' | 'Draw';

export interface GameState {
  gameId: string;
  playerXId: string;
  playerOId: string;
  cells: CellEnum[];
  nextPlayer: 'X' | 'O';
  status: GameStatusWire;
  version: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateGameDto {
  playerXId: string;
  playerOId: string;
}

export interface MakeMoveDto {
  cellIndex: number;
  version: number;
}

export const renderCell = (c: CellEnum) =>
  c === 1 ? 'X' : c === 2 ? 'O' : '';

export const isInProgress = (s: GameStatusWire) =>
  s === 0 || s === 'InProgress';

export const statusText = (s: GameStatusWire) => {
  if (s === 0 || s === 'InProgress') return 'U toku';
  if (s === 1 || s === 'XWon') return 'Pobednik: X';
  if (s === 2 || s === 'OWon') return 'Pobednik: O';
  if (s === 3 || s === 'Draw') return 'Nerešeno';
  return String(s);
};
