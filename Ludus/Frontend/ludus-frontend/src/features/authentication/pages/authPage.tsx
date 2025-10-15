import React, { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Modal } from 'antd'
import LoginForm from '../ui/loginForm'
import RegisterForm from '../ui/registrationForm'
import Verify2FAModal from '../ui/verify2fa'
import { useAuth } from '../model/useAuth'
import '../pages/auth.css'

const isValidJMBG = (jmbg: string): boolean => {
  if (!jmbg || jmbg.trim().length !== 13) return false
  if (!/^\d{13}$/.test(jmbg)) return false

  const day = parseInt(jmbg.substring(0, 2), 10)
  const month = parseInt(jmbg.substring(2, 4), 10)
  let year = parseInt(jmbg.substring(4, 7), 10)

  if (isNaN(day) || isNaN(month) || isNaN(year)) return false

  if (year >= 900) year = 1000 + year
  else year = 2000 + year

  const date = new Date(year, month - 1, day)
  if (date.getFullYear() !== year || date.getMonth() + 1 !== month || date.getDate() !== day) {
    return false
  }

  return true
}

const AuthPage: React.FC = () => {
  const { register, login, verify2FA, loading, error } = useAuth()
  const navigate = useNavigate()

  const [mode, setMode] = useState<'login' | 'register'>('login')
  const [emailFor2FA, setEmailFor2FA] = useState<string | null>(null)
  const [showVerify, setShowVerify] = useState(false)
  const [verifyError, setVerifyError] = useState<string | null>(null)
  const [verifyLoading, setVerifyLoading] = useState(false)

  const [modalVisible, setModalVisible] = useState(false)
  const [modalTitle, setModalTitle] = useState('')
  const [modalMessage, setModalMessage] = useState('')
  const [modalType, setModalType] = useState<'success' | 'error'>('success')

  const showModal = (title: string, msg: string, type: 'success' | 'error', callback?: () => void) => {
    setModalTitle(title)
    setModalMessage(msg)
    setModalType(type)
    setModalVisible(true)
    if (type === 'success' && callback) {
      setTimeout(() => {
        setModalVisible(false)
        callback()
      }, 2000)
    }
  }

  const handleModalClose = () => setModalVisible(false)

  // --- LOGIN ---
  const handleLogin = async (email: string, password: string) => {
    const resp = await login({ email, password })
    console.log('Login response:', resp)

    if (resp?.requires2FA) {
      setEmailFor2FA(email)
      setShowVerify(true)
    } else if (resp?.token) {
      localStorage.setItem('token', resp.token)
      window.dispatchEvent(new Event('authChanged'))
      showModal('Success', 'Login successful! Redirecting...', 'success', () => navigate('/dashboard'))
    } else {
      const errorMessage =
        resp?.message === 'Request failed with status code 401'
          ? 'Invalid credentials. Please check your email and password.'
          : resp?.message || 'Login failed. Please try again.'
      showModal('Login Failed', errorMessage, 'error')
    }
  }

  const handleVerify = async (code: string) => {
    if (!emailFor2FA) return
    setVerifyError(null)
    setVerifyLoading(true)
    try {
      const resp = await verify2FA({ email: emailFor2FA, code })
      console.log('Verify2FA response:', resp)

      if (resp?.token) {
        localStorage.setItem('token', resp.token)
        window.dispatchEvent(new Event('authChanged'))

        showModal('Success', 'Login successful!', 'success', () => {
          setShowVerify(false)
          navigate('/dashboard')
        })
      } else {
        const errorMsg = resp?.message ?? 'Verification failed. Please try again.'
        setVerifyError(errorMsg)
        showModal('Error', errorMsg, 'error')
      }
    } catch {
      setVerifyError('Verification failed. Please try again.')
      showModal('Error', 'Verification failed. Please try again.', 'error')
    } finally {
      setVerifyLoading(false)
    }
  }

  const handleCancelVerify = () => setShowVerify(false)

  const handleRegister = async (data: {
    email: string
    password: string
    confirmPassword: string
    mlb: string
    firstName: string
    lastName: string
  }) => {
    if (data.password !== data.confirmPassword) {
      showModal('Password Mismatch', 'Passwords do not match. Please try again.', 'error')
      return
    }

    if (!isValidJMBG(data.mlb)) {
      showModal('Invalid MLB', 'Invalid MLB number. Please enter a valid 13-digit number.', 'error')
      return
    }

    const resp = await register({
      email: data.email,
      password: data.password,
      confirmPassword: data.confirmPassword,
      mlb: data.mlb,
      firstName: data.firstName,
      lastName: data.lastName
    })

    console.log('🔍 Register response (FULL):', resp)

    if (resp?.success) {
      showModal('Successful Registration', 'Your account has been created successfully!', 'success', () => {
        setMode('login')
        navigate('/auth')
      })
      return
    }

    let errorMessage = 'Registration failed. Please try again.'

    if (typeof resp === 'string') {
          const respStr = resp as string

      errorMessage = respStr.includes('Request failed with status code 400')
        ? 'User with this email already exists.'
        : respStr
    } else if (Array.isArray(resp)) {
      const duplicate = resp.find((e: any) => e.code === 'DuplicateUserName' || e.code === 'DuplicateEmail')
      errorMessage = duplicate
        ? 'User with this email is already registered.'
        : resp.map((e: any) => e.description).join(' ')
    } else if (resp?.message) {
      if (Array.isArray(resp.message)) {
        const duplicate = resp.message.find((e: any) => e.code === 'DuplicateUserName' || e.code === 'DuplicateEmail')
        errorMessage = duplicate
          ? 'User with this email is already registered.'
          : resp.message.map((e: any) => e.description).join(' ')
      } else if (typeof resp.message === 'string') {
        errorMessage = resp.message.includes('Request failed with status code 400')
          ? 'User with this email already exists.'
          : resp.message
      } else if (typeof resp.message === 'object' && resp.message !== null) {
        const msgObj = resp.message as any
        errorMessage = msgObj.description || msgObj.error || msgObj.message || JSON.stringify(resp.message)
      }
    }

    showModal('Registration Failed', errorMessage, 'error')
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

        <Modal
          title={modalTitle}
          open={modalVisible}
          onOk={handleModalClose}
          onCancel={handleModalClose}
          centered
          okText="OK"
          cancelButtonProps={{ style: { display: 'none' } }}
        >
          {modalMessage && (
            <p
              style={{
                color: modalType === 'success' ? '#52c41a' : '#ff4d4f',
                fontSize: '16px',
                fontWeight: '500'
              }}
            >
              {modalMessage}
            </p>
          )}
        </Modal>
      </div>
    </div>
  )
}

export default AuthPage
