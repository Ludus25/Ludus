import React, { useEffect, useState } from 'react';
import { Button } from 'antd';
import { useLocation, useNavigate } from 'react-router-dom';
import { isLoggedIn, logout } from '../../../shared/utils/auth.ts';

const Header: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const [isAuthenticated, setIsAuthenticated] = useState(isLoggedIn());

  useEffect(() => {
    setIsAuthenticated(isLoggedIn());
  }, [location]);

  const handleLogout = () => {
    logout();
    navigate('/auth'); // Preusmeravanje na stranicu za prijavu
  };

  return (
    <header style={headerStyle}>
      <div style={logoStyle}>Ludus</div>
      {isAuthenticated && (
        <Button type="primary" danger onClick={handleLogout}>
          Logout
        </Button>
      )}
    </header>
  );
};

const headerStyle: React.CSSProperties = {
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'center',
  padding: '12px 24px',
  backgroundColor: '#ffffff',
  borderBottom: '1px solid #e8e8e8',
};

const logoStyle: React.CSSProperties = {
  fontSize: '1.5rem',
  fontWeight: 'bold',
};

export default Header;
