import { http } from '../../../shared/api/http'
import type { JoinMatchRequest, MatchStatus, QueueStatus } from '../model/types'

export const matchmakingApi = {
  // POST /api/matchmaking/join
  join(dto: JoinMatchRequest) {
    return http.post<{ message: string }>('/matchmaking/join', dto).then(r => r.data)
  },

  // GET /api/matchmaking/status/{playerId}
  getStatus(playerId: string) {
    return http.get<MatchStatus>(`/matchmaking/status/${playerId}`).then(r => r.data)
  },

  // GET /api/matchmaking/queue
  getQueue() {
    return http.get<QueueStatus[]>('/matchmaking/queue').then(r => r.data)
  }
}