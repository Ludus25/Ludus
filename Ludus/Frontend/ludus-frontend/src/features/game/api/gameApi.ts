import { http } from '../../../shared/api/http'
import type { CreateGameDto, GameState, MakeMoveDto } from '../model/types'

export const gameApi = {
  // POST  /xo-game
  create(dto: CreateGameDto) {
    return http.post<GameState>('/xo-game', dto).then(r => r.data)
  },

  // GET  /xo-game/{id}
  get(id: string) {
    return http.get<GameState>(`/xo-game/${id}`).then(r => r.data)
  },

  // POST  /xo-game/{id}/move
  move(id: string, body: MakeMoveDto, actingUserId: string) {
    return http.post<GameState>(`/xo-game/${id}/move`, body, {
      headers: { 'X-UserId': actingUserId }
    }).then(r => r.data)
  }
}
