import { useState } from 'react'
import { authApi } from '../api/authApi'
import type { RegisterResponse, LoginResponse, Verify2FAResponse } from './types'

export function useAuth() {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function register(dto: Parameters<typeof authApi.register>[0]): Promise<RegisterResponse> {
    setError(null)
    setLoading(true)
    try {
      return await authApi.register(dto)
    } catch (err: any) {
      const msg = err?.response?.data ?? err.message
      return { success: false, message: msg }
    } finally {
      setLoading(false)
    }
  }

  async function login(dto: Parameters<typeof authApi.login>[0]): Promise<LoginResponse> {
    setError(null)
    setLoading(true)
    try {
      return await authApi.login(dto)
    } catch (err: any) {
      const msg = err?.response?.data ?? err.message
      return { success: false, message: msg }
    } finally {
      setLoading(false)
    }
  }

  async function verify2FA(dto: Parameters<typeof authApi.verify2FA>[0]): Promise<Verify2FAResponse> {
    setError(null)
    setLoading(true)
    try {
      return await authApi.verify2FA(dto)
    } catch (err: any) {
      const msg = err?.response?.data ?? err.message
      return { success: false, message: msg }
    } finally {
      setLoading(false)
    }
  }

  return {
    loading,
    error,
    register,
    login,
    verify2FA
  }
}
