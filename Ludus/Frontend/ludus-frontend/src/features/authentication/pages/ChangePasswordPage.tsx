import React, { useState } from 'react'
import ChangePasswordForm from '../ui/ChangePasswordForm'
import { changePassword } from '../api/authApi'

const ChangePasswordPage: React.FC = () => {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const handleChangePassword = async (data: {
    email: string
    oldPassword: string
    newPassword: string
    confirmPassword: string
  }) => {
    setLoading(true)
    setError(null)
    try {
      const response = await changePassword(data)
      alert(response.message)
    } catch (err: any) {
      setError(err.response?.data || 'Something went wrong')
    } finally {
      setLoading(false)
    }
  }

  return (
    <ChangePasswordForm
      onSubmit={handleChangePassword}
      loading={loading}
      error={error}
    />
  )
}

export default ChangePasswordPage
