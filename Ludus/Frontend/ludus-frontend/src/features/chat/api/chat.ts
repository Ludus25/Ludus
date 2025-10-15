import { http } from '../../../shared/api/http'
import type { ChatMessage, SendMessageRequest } from '../model/types'

export const chatApi = {
  // POST /api/chat/send (ako postoji)
  sendMessage(dto: SendMessageRequest) {
    return http.post<{ success: boolean }>('/api/chat/send', dto)
      .then(r => r.data)
  },

  // GET /api/chat/messages/{gameId}
  getMessages(gameId: string) {
    return http.get<ChatMessage[]>(`/api/chat/messages/${gameId}`)
      .then(r => r.data)
  }
}