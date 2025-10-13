import { BrowserRouter, Routes, Route, Navigate, useLocation } from "react-router-dom";
import GamePage from "../features/game/pages/GamePage";
import DashboardPage from "../features/dashboard/pages/DashboardPage";
import MatchmakingPage from "../features/matchmaking/pages/MatchmakingPage";
import { isLoggedIn } from '../shared/utils/auth.ts'
import AuthPage from "../features/authentication/pages/authPage"; 
import Header from '../features/authentication/ui/header.tsx'

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
        
        <h1>Ludus Frontend</h1>
        
        <Routes>
         
          <Route path="/" element={<Navigate to="/auth" />} />

          <Route path="/auth" element={<AuthPage />} />
          <Route path="/dashboard"   element={isLoggedIn() ? <DashboardPage /> : <Navigate to="/auth" />} />
          <Route path="/game" element={<GamePage />} />
          <Route path="/matchmaking" element={<MatchmakingPage />} />

        
          <Route path="*" element={<Navigate to="/auth" />} />
        </Routes>
      </div>
    </BrowserRouter>
  );
}
