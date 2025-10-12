export interface JoinMatchRequest {
  playerId: string;
  rating: number;
}

export interface MatchStatus {
  status: 'not_found' | 'searching' | 'matched';
  matchId?: string;
  players?: string[];
}

export interface QueueStatus {
  playerId: string;
  rating: number;
}