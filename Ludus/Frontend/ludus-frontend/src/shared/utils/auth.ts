import {jwtDecode} from 'jwt-decode'

interface JwtPayload {
  exp: number
}

export const getToken = () => localStorage.getItem('token')

//export const isLoggedIn = () => !!getToken()
export function isLoggedIn(): boolean {
  const token = localStorage.getItem('token');
  return !!token; // Vraća true ako token postoji
}

export const logout = () => {
  localStorage.removeItem('token')
  window.location.href = '/auth'
}

export const saveToken = (token: string) => localStorage.setItem('token', token)

export const clearAuth = () => localStorage.removeItem('token')

export const isTokenExpired = () => {
  const token = localStorage.getItem('token')
  if (!token) return true

  try {
    const decoded = jwtDecode<JwtPayload>(token)
    return decoded.exp * 1000 < Date.now()
  } catch {
    return true 
  }
}