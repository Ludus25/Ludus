import React, { useState } from 'react'

interface RegisterFormProps {
  onSubmit: (data: {
    email: string
    password: string
    confirmPassword: string
    mlb: string
    firstName: string
    lastName: string
  }) => void
  loading: boolean
  error: string | null
}

const RegisterForm: React.FC<RegisterFormProps> = ({ onSubmit, loading, error }) => {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [mlb, setMlb] = useState('')
  const [firstName, setFirstName] = useState('')
  const [lastName, setLastName] = useState('')

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()

    // log za debug pre slanja
    const payload = { email, password, confirmPassword, mlb, firstName, lastName, enable2FA: false }
    console.log('Register payload:', payload)

    onSubmit(payload)
  }

  return (
    <form className="auth-form" onSubmit={handleSubmit}>
      {error && <div className="error-message">{error}</div>}
      <input
        type="email"
        placeholder="Email"
        value={email}
        onChange={e => setEmail(e.target.value)}
        required
      />
      <input
        type="password"
        placeholder="Password"
        value={password}
        onChange={e => setPassword(e.target.value)}
        required
      />
      <input
        type="password"
        placeholder="Confirm Password"
        value={confirmPassword}
        onChange={e => setConfirmPassword(e.target.value)}
        required
      />
      <input
        type="text"
        placeholder="MLB"
        value={mlb}
        onChange={e => setMlb(e.target.value)}
        required
      />
      <input
        type="text"
        placeholder="First Name"
        value={firstName}
        onChange={e => setFirstName(e.target.value)}
        required
      />
      <input
        type="text"
        placeholder="Last Name"
        value={lastName}
        onChange={e => setLastName(e.target.value)}
        required
      />
      <button type="submit" disabled={loading}>
        Sign Up
      </button>
    </form>
  )
}

export default RegisterForm
