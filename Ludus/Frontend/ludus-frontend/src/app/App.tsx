import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import GamePage from "../features/game/pages/GamePage";
import DashboardPage from "../features/dashboard/pages/DashboardPage";
import MatchmakingPage from "../features/matchmaking/pages/MatchmakingPage";
import { isLoggedIn } from '../shared/utils/auth.ts'
import AuthPage from "../features/authentication/pages/authPage";
import Header from '../features/authentication/ui/header.tsx'
import React, { useState, useEffect } from 'react';
import UsersPage from '../features/admin/pages/userPages.tsx';
import AdminRoute from '../features/admin/route/adminRoute.tsx';



const ProtectedRoute: React.FC<{ element: React.ReactElement }> = ({ element }) => {
  const [isAuth, setIsAuth] = useState(isLoggedIn());

  useEffect(() => {
    
    const handleStorageChange = () => {
      setIsAuth(isLoggedIn());
    };

    window.addEventListener('storage', handleStorageChange);
    
    
    window.addEventListener('authChanged', handleStorageChange);

    return () => {
      window.removeEventListener('storage', handleStorageChange);
      window.removeEventListener('authChanged', handleStorageChange);
    };
  }, []);

  return isAuth ? element : <Navigate to="/auth" />;
};

export default function App() {
  return (
    <BrowserRouter>
      <div
        style={{
          maxWidth: 960,
          margin: "24px auto",
          fontFamily: "system-ui, sans-serif",
        }}
      >
        <Header />
        <h1>Ludus Frontend</h1>

        <Routes>
          <Route path="/" element={<Navigate to="/auth" />} />
          <Route path="/auth" element={<AuthPage />} />
          <Route path="/dashboard" element={<ProtectedRoute element={<DashboardPage />} />} />
          <Route path="/" element={<Navigate to="/dashboard" />} />
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route path="/game/:gameId" element={<GamePage />} />
          <Route path="/game" element={<GamePage />} />
          <Route path="/matchmaking" element={<MatchmakingPage />} />
          <Route path="/users" element={<AdminRoute element={<UsersPage />} />} />
          <Route path="*" element={<Navigate to="/auth" />} />
        </Routes>
      </div>
    </BrowserRouter>
  );
}