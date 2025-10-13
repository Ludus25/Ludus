

export interface RegisterResponse {
  success: boolean
  message?: string
}

export interface LoginResponse {
  success?: boolean
  requires2FA?: boolean
  token?: string
  message?: string
}

export interface Verify2FAResponse {
  success: boolean
  token?: string
  message?: string
}

export interface RegisterRequest {
  email: string
  password: string
  confirmPassword: string
  mlb: string
  firstName: string
  lastName: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface Verify2FARequest {
  email: string
  code: string
}
export interface ChangePasswordRequest {
  email: string;
  oldPassword: string;
  newPassword: string;
  confirmPassword: string;
}

export interface ChangePasswordResponse {
  message: string;
}