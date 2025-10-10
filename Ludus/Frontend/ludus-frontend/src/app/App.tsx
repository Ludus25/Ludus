import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import GamePage from "../features/game/pages/GamePage";
import DashboardPage from "../features/dashboard/pages/DashboardPage";

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
          <Route path="/" element={<Navigate to="/dashboard" />} />
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route path="/game" element={<GamePage />} />
        </Routes>
      </div>
    </BrowserRouter>
  );
}
