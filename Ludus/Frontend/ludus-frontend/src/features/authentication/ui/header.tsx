import React from 'react'
import { Button } from 'antd'
import { isLoggedIn, logout } from '../../../shared/utils/auth.ts'

const Header: React.FC = () => {
  return (
    <header style={headerStyle}>
      <div style={logoStyle}>Ludus</div>
      {isLoggedIn() && (
        <Button type="primary" danger onClick={logout}>
          Logout
        </Button>
      )}
    </header>
  )
}

const headerStyle: React.CSSProperties = {
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'center',
  padding: '12px 24px',
  backgroundColor: '#ffffff',
  borderBottom: '1px solid #e8e8e8',
}

const logoStyle: React.CSSProperties = {
  fontSize: '1.5rem',
  fontWeight: 'bold',
}

export default Header
