import React, { useState } from 'react'

interface Props {
  email: string
  onVerify: (code: string) => void
  onCancel: () => void
  loading: boolean
  error: string | null
}

const Verify2FAModal: React.FC<Props> = ({
  email,
  onVerify,
  onCancel,
  loading,
  error
}) => {
  const [code, setCode] = useState('')

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    onVerify(code)
  }

  return (
    <div style={modalOverlayStyle}>
      <div style={modalContentStyle}>
        <h3>Enter verification code sent to {email}</h3>
        {error && <div style={{ color: 'red' }}>{error}</div>}
        <form onSubmit={handleSubmit}>
          <input
            type="text"
            value={code}
            onChange={e => setCode(e.target.value)}
            required
          />
          <div style={{ marginTop: '12px' }}>
            <button type="submit" disabled={loading}>
              Confirm
            </button>
            <button type="button" onClick={onCancel} disabled={loading}>
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}

export default Verify2FAModal

const modalOverlayStyle: React.CSSProperties = {
  position: 'fixed',
  top: 0,
  left: 0,
  right: 0,
  bottom: 0,
  backgroundColor: 'rgba(0,0,0,0.5)',
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center'
}

const modalContentStyle: React.CSSProperties = {
  backgroundColor: 'white',
  padding: '20px',
  borderRadius: '8px',
  width: '300px',
  textAlign: 'center'
}
