import { http } from '../../../shared/api/http'

export interface GameHistoryListItem {
  matchId: string
  playerUserIds: string[]
  playerEmails?: string[]
  startedAt: string
  endedAt: string
  winnerUserId?: string | null
}

export interface GameHistoryFull extends GameHistoryListItem {
  moveHistory: string
}

export const fetchGamesByEmail = async (email: string, limit = 1000, offset = 0) => {
 const { data } = await http.get<GameHistoryListItem[]>(`/game-history/email/${encodeURIComponent(email)}?limit=${limit}&offset=${offset}`)
  return data
}

export const fetchGameByMatchId = async (matchId: string) => {
 const { data } = await http.get<GameHistoryFull>(`/game-history/match/${encodeURIComponent(matchId)}`)
  return data
}
