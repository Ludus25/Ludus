import { useNavigate } from "react-router-dom";

export default function DashboardPage() {
  const navigate = useNavigate();

  return (
    <div style={{ textAlign: "center", marginTop: "100px" }}>
      <h1>Welcome to Ludus</h1>
      <p>Click below to start the game</p>
      <button
        onClick={() => navigate("/game")}
        style={{
          padding: "10px 20px",
          fontSize: "16px",
          marginTop: "20px",
          cursor: "pointer",
        }}
      >
        Start Game
      </button>

      <button onClick={() => navigate("/matchmaking")}>
        Find Match
      </button>
      <button onClick={() => navigate("/chat")} style={{ marginLeft: 10 }}>
  	Chat
      </button>
    </div>
  );
}
