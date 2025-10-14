import React, { useEffect, useState } from 'react';
import { Button } from 'antd';
import { useLocation, useNavigate } from 'react-router-dom';
import { isLoggedIn, logout, getUserRole } from '../../../shared/utils/auth.ts';

const Header: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const [isAuthenticated, setIsAuthenticated] = useState(isLoggedIn());
  const [isAdmin, setIsAdmin] = useState(false);

  useEffect(() => {
    setIsAuthenticated(isLoggedIn());
    const role = getUserRole();
    setIsAdmin(role === 'Admin');
  }, [location]);

  const handleLogout = () => {
    logout();
    navigate('/auth');
  };

  const handleViewUsers = () => {
    navigate('/users'); 
  };

  return (
    <header style={headerStyle}>
      <div style={logoStyle}>Ludus</div>

      <div style={{ display: 'flex', gap: '10px' }}>
        {isAuthenticated && isAdmin && (
          <Button type="default" onClick={handleViewUsers}>
            Users
          </Button>
        )}

        {isAuthenticated && (
          <Button type="primary" danger onClick={handleLogout}>
            Logout
          </Button>
        )}
      </div>
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
