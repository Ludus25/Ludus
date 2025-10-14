import React from 'react';
import { Navigate } from 'react-router-dom';
import { isLoggedIn, getUserRole } from '../../../shared/utils/auth.ts';

const AdminRoute: React.FC<{ element: React.ReactElement }> = ({ element }) => {
  const isAuth = isLoggedIn();
  const role = getUserRole();

  if (!isAuth) return <Navigate to="/auth" />;
  if (role !== 'Admin') return <Navigate to="/dashboard" />; 

  return element;
};

export default AdminRoute;
