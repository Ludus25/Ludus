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
    <div style={overlay}>
      <div style={content}>
        <h2 style={titleStyle}>Verify Your Account</h2>
        <p style={subtitleStyle}>
          Enter the verification code sent to <strong>{email}</strong>
        </p>
        {error && <div style={errorStyle}>{error}</div>}
        <form onSubmit={handleSubmit}>
          <input
            type="text"
            value={code}
            onChange={e => setCode(e.target.value)}
            required
            autoFocus
            style={inputStyle}
            placeholder="Enter code"
          />
          <div style={buttonsContainer}>
            <button
              type="submit"
              disabled={loading}
              style={{
                ...buttonStyle,
                ...(loading ? buttonDisabledStyle : buttonPrimaryStyle),
              }}
            >
              {loading ? 'Verifying...' : 'Confirm'}
            </button>
            <button
              type="button"
              onClick={onCancel}
              disabled={loading}
              style={{
                ...buttonStyle,
                ...buttonCancelStyle,  // ispravno spajanje CSS objekata
              }}
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}

export default Verify2FAModal

// --- Stilovi ---

const overlay: React.CSSProperties = {
  position: 'fixed',
  top: 0,
  left: 0,
  right: 0,
  bottom: 0,
  backgroundColor: 'rgba(0, 0, 0, 0.5)',
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  zIndex: 1000,
}

const content: React.CSSProperties = {
  backgroundColor: '#fff',
  padding: '24px',
  borderRadius: '8px',
  width: '320px',
  boxShadow: '0 2px 10px rgba(0,0,0,0.2)',
  textAlign: 'center',
}

const titleStyle: React.CSSProperties = {
  margin: 0,
  marginBottom: '16px',
  fontSize: '1.5rem',
  color: '#333',
}

const subtitleStyle: React.CSSProperties = {
  margin: 0,
  marginBottom: '24px',
  fontSize: '1rem',
  color: '#555',
}

const errorStyle: React.CSSProperties = {
  color: 'red',
  marginBottom: '16px',
  fontSize: '0.9rem',
}

const inputStyle: React.CSSProperties = {
  width: '100%',
  padding: '8px 12px',
  fontSize: '1rem',
  border: '1px solid #ccc',
  borderRadius: '4px',
  boxSizing: 'border-box',
}

const buttonsContainer: React.CSSProperties = {
  marginTop: '24px',
  display: 'flex',
  justifyContent: 'space-between',
}

const buttonStyle: React.CSSProperties = {
  flex: 1,
  padding: '10px 16px',
  fontSize: '1rem',
  borderRadius: '4px',
  border: 'none',
  cursor: 'pointer',
  margin: '0 4px',
}

const buttonPrimaryStyle: React.CSSProperties = {
  backgroundColor: '#1890ff',
  color: '#fff',
}

const buttonCancelStyle: React.CSSProperties = {
  backgroundColor: '#f5f5f5',
  color: '#333',
}

const buttonDisabledStyle: React.CSSProperties = {
  backgroundColor: '#a0cfff',
  cursor: 'not-allowed',
}
