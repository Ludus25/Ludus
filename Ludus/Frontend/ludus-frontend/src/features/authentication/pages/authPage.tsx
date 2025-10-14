import React, { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Modal } from 'antd'
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

  const handleModalClose = () => {
    setModalVisible(false)
  }

  const handleLogin = async (email: string, password: string) => {
    const resp = await login({ email, password })
    console.log('Login response:', resp)

    if (resp?.requires2FA) {
      setEmailFor2FA(email)
      setShowVerify(true)
    } else {
      showModal('Error', resp?.message ?? '2FA is required. Please try login again.', 'error')
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
        
        showModal(
          'Success',
          'Login successful!',
          'success',
          () => {
            setShowVerify(false)
            navigate('/dashboard')
          }
        )
      } else {
        const errorMsg = resp?.message ?? 'Verification failed. Please try again.'
        setVerifyError(errorMsg)
        showModal('Error', errorMsg, 'error')
      }
    } catch (err) {
      setVerifyError('Verification failed. Please try again.')
      showModal('Error', 'Verification failed. Please try again.', 'error')
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

    if (resp?.success) {
      showModal(
        'Successful Registration',
        '',
        'success',
        () => {
          setMode('login')
          navigate('/auth')
        }
      )
    } else {
      showModal('Failed Registration', '', 'error')
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

        <Modal
          title={modalTitle}
          open={modalVisible}
          onOk={handleModalClose}
          onCancel={handleModalClose}
          centered
          okText="OK"
          cancelButtonProps={{ style: { display: 'none' } }}
        >
          <p style={{
            color: modalType === 'success' ? '#52c41a' : '#ff4d4f',
            fontSize: '16px',
            fontWeight: '500'
          }}>
            {modalMessage}
          </p>
        </Modal>
      </div>
    </div>
  )
}

export default AuthPage