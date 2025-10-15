import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom"
import GamePage from "../features/game/pages/GamePage"
import DashboardPage from "../features/dashboard/pages/DashboardPage"
import MatchmakingPage from "../features/matchmaking/pages/MatchmakingPage"
import ChatPage from "../features/chat/pages/ChatPage"; 
import AuthPage from "../features/authentication/pages/authPage"
import Header from '../features/authentication/ui/header'
import { isLoggedIn } from '../shared/utils/auth'
import React, { useState, useEffect } from 'react'

const ProtectedRoute: React.FC<{ element: React.ReactElement }> = ({ element }) => {
  const [isAuth, setIsAuth] = useState(isLoggedIn())

  useEffect(() => {
    const handleStorageChange = () => {
      setIsAuth(isLoggedIn())
    }

    window.addEventListener('storage', handleStorageChange)
    window.addEventListener('authChanged', handleStorageChange)

    return () => {
      window.removeEventListener('storage', handleStorageChange)
      window.removeEventListener('authChanged', handleStorageChange)
    }
  }, [])

  return isAuth ? element : <Navigate to="/auth" replace />
}

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

        <Routes>
          <Route path="/" element={<Navigate to="/auth" replace />} />
          <Route path="/auth" element={<AuthPage />} />

          <Route 
            path="/dashboard" 
            element={<ProtectedRoute element={<DashboardPage />} />} 
          />
          <Route 
            path="/matchmaking" 
            element={<ProtectedRoute element={<MatchmakingPage />} />} 
          />
          <Route 
            path="/game/:gameId" 
            element={<ProtectedRoute element={<GamePage />} />} 
          />
          <Route 
            path="/game" 
            element={<ProtectedRoute element={<GamePage />} />} 
          />

	  <Route 
	    path="/chat" 
	    element={<ProtectedRoute element={<ChatPage />} />} 
	  />

          <Route path="*" element={<Navigate to="/auth" replace />} />
        </Routes>
      </div>
    </BrowserRouter>
  )
}