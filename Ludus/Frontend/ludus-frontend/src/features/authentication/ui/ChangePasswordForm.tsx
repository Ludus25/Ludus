import React, { useState } from 'react'

interface Props {
  onSubmit: (data: {
    email: string
    oldPassword: string
    newPassword: string
    confirmPassword: string
  }) => void
  loading: boolean
  error: string | null
}

const ChangePasswordForm: React.FC<Props> = ({ onSubmit, loading, error }) => {
  const [email, setEmail] = useState('')
  const [oldPassword, setOldPassword] = useState('')
  const [newPassword, setNewPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    onSubmit({ email, oldPassword, newPassword, confirmPassword })
  }

  return (
    <div className="flex justify-center items-center min-h-screen bg-gray-100">
      <form
        onSubmit={handleSubmit}
        className="bg-white p-6 rounded-xl shadow-md w-96"
      >
        <h2 className="text-2xl font-semibold mb-4 text-blue-600 text-center">
          Change Password
        </h2>

        {error && <p className="text-red-500 text-center mb-2">{error}</p>}

        <input
          type="email"
          placeholder="Email"
          className="w-full mb-3 border rounded-lg p-2"
          value={email}
          onChange={e => setEmail(e.target.value)}
          required
        />

        <input
          type="password"
          placeholder="Old Password"
          className="w-full mb-3 border rounded-lg p-2"
          value={oldPassword}
          onChange={e => setOldPassword(e.target.value)}
          required
        />

        <input
          type="password"
          placeholder="New Password"
          className="w-full mb-3 border rounded-lg p-2"
          value={newPassword}
          onChange={e => setNewPassword(e.target.value)}
          required
        />

        <input
          type="password"
          placeholder="Confirm New Password"
          className="w-full mb-4 border rounded-lg p-2"
          value={confirmPassword}
          onChange={e => setConfirmPassword(e.target.value)}
          required
        />

        <button
          type="submit"
          className="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition"
          disabled={loading}
        >
          {loading ? 'Changing...' : 'Change Password'}
        </button>
      </form>
    </div>
  )
}

export default ChangePasswordForm
