import { http } from '../../../shared/api/http'
import type { RegisterResponse, LoginResponse, Verify2FAResponse } from '../model/types'
import type { ChangePasswordRequest, ChangePasswordResponse } from '../model/types'

interface RegisterDTO {
  email: string
  password: string
  confirmPassword: string
  mlb: string
  firstName: string
  lastName: string
}

interface LoginDTO {
  email: string
  password: string
}

interface Verify2FADTO {
  email: string
  code: string
}

export const authApi = {
  register(dto: RegisterDTO) {
    const payload = {
      Email: dto.email,
      Password: dto.password,
      ConfirmPassword: dto.confirmPassword,
      Mlb: dto.mlb,
      Name: dto.firstName,
      Surname: dto.lastName,
      Enable2FA: true
    }
    console.log('Register payload:', payload)
    return http.post<RegisterResponse>('/auth/register', payload).then(res => res.data)
  },

  login(dto: LoginDTO) {
    const payload = {
      Email: dto.email,
      Password: dto.password
    }
    console.log('Login payload:', payload)
    return http.post<LoginResponse>('/auth/login', payload).then(res => res.data)
  },

  verify2FA(dto: Verify2FADTO) {
    const payload = {
      Email: dto.email,
      Code: dto.code,
      RememberMe: false
    }
    console.log('Verify2FA payload:', payload)
    return http.post<Verify2FAResponse>('/auth/verify-2fa', payload).then(res => res.data)
  }
}


export const changePassword = async (data: ChangePasswordRequest): Promise<ChangePasswordResponse> => {
  const response = await http.put('/auth/change-password', data)
  return response.data
}
