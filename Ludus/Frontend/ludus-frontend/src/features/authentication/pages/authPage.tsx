import React, { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import LoginForm from '../ui/loginForm'
import RegisterForm from '../ui/registrationForm'
import Verify2FAModal from '../ui/verify2fa'
import { useAuth } from '../model/useAuth'
import '../pages/auth.css'

const AuthPage: React.FC = () => {
  const { register, login, verify2FA, loading, error } = useAuth()
  const navigate = useNavigate()
  const [mode, setMode] = useState<'login' | 'register'>('login')
  const [emailFor2FA, setEmailFor2FA] = useState<string | null>(null)
  const [showVerify, setShowVerify] = useState(false)
  const [verifyError, setVerifyError] = useState<string | null>(null)
  const [verifyLoading, setVerifyLoading] = useState(false)

  // --- LOGIN (uvek ide na 2FA) ---
  const handleLogin = async (email: string, password: string) => {
    const resp = await login({ email, password })
    console.log('Login response:', resp)

    if (resp?.requires2FA) {
      // Čekamo verifikaciju
      setEmailFor2FA(email)
      setShowVerify(true)
    } else {
      // Ako login API ne vraća requires2FA, tretiraj to kao grešku
      alert('2FA is required. Please try login again.')
    }
  }

  // --- VERIFY (posle unosa 2FA koda) ---
  const handleVerify = async (code: string) => {
    if (!emailFor2FA) return
    setVerifyError(null)
    setVerifyLoading(true)
    try {
      const resp = await verify2FA({ email: emailFor2FA, code })
      console.log('Verify2FA response:', resp)

      if (resp?.token) {
        // Uspešna verifikacija
        localStorage.setItem('token', resp.token)
        
        // Dispečuj event da bi se ProtectedRoute components ažurirao
        window.dispatchEvent(new Event('authChanged'))
        
        setShowVerify(false)
        navigate('/dashboard')
      } else {
        setVerifyError(resp.message ?? 'Verification failed')
      }
    } catch (err) {
      setVerifyError('Verification failed')
    } finally {
      setVerifyLoading(false)
    }
  }

  const handleCancelVerify = () => {
    setShowVerify(false)
  }

  // --- REGISTER ---
  const handleRegister = async (data: {
    email: string
    password: string
    confirmPassword: string
    mlb: string
    firstName: string
    lastName: string
  }) => {
    const resp = await register({
      email: data.email,
      password: data.password,
      confirmPassword: data.confirmPassword,
      mlb: data.mlb,
      firstName: data.firstName,
      lastName: data.lastName
    })

    if (resp.success) {
      alert('Registration successful! Please log in.')
      setMode('login')
      navigate('/auth')
    } else {
      console.error('Registration error:', resp.message)
      alert(resp.message ?? 'Registration failed')
    }
  }

  return (
    <div className="auth-wrapper">
      <div className="auth-box">
        <h1>{mode === 'login' ? 'Login' : 'Create Account'}</h1>

        {mode === 'login' ? (
          <>
            <LoginForm onSubmit={handleLogin} loading={loading} error={error} />
            <div className="create-account">
              <button type="button" onClick={() => setMode('register')}>
                Create new account
              </button>
            </div>
          </>
        ) : (
          <>
            <RegisterForm onSubmit={handleRegister} loading={loading} error={error} />
            <div className="create-account">
              <button type="button" onClick={() => setMode('login')}>
                Back to login
              </button>
            </div>
          </>
        )}

        {showVerify && emailFor2FA && (
          <Verify2FAModal
            email={emailFor2FA}
            onVerify={handleVerify}
            onCancel={handleCancelVerify}
            loading={verifyLoading}
            error={verifyError}
          />
        )}
      </div>
    </div>
  )
}

export default AuthPage