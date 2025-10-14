import { http } from '../../../shared/api/http'

export interface UserDTO {
  id: string
  email: string
  firstName: string
  lastName: string
}

export const adminApi = {
  getAllUsers: async (): Promise<UserDTO[]> => {
    const token = localStorage.getItem('token') 
    const response = await http.get('/test/names', {
      headers: { Authorization: `Bearer ${token}` }
    })
    return response.data
  }
}
