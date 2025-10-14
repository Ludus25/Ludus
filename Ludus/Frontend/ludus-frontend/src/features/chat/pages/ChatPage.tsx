import { useState, useEffect } from "react";
import { Card, Button, Input, Typography, Space, Alert, Divider } from "antd";
import { useChat } from "../model/useChat";

const { Title } = Typography;

type OnlineUsersProps = { users: string[] };

const OnlineUsers: React.FC<OnlineUsersProps> = ({ users }) => (
  <Space direction="vertical" size="small" style={{ width: "100%" }}>
    {users.length === 0 ? (
      <div style={{ color: "#888" }}>Nema online korisnika</div>
    ) : (
      users.map((u, idx) => (
        <div
          key={idx}
          style={{
            display: "flex",
            alignItems: "center",
            padding: "4px 8px",
            backgroundColor: "#e3f2fd",
            borderRadius: 6,
          }}
        >
          <div
            style={{
              width: 10,
              height: 10,
              borderRadius: "50%",
              backgroundColor: "green",
              marginRight: 8,
            }}
          />
          <span>{u}</span>
        </div>
      ))
    )}
  </Space>
);

export default function ChatPage() {
  const [usernameInput, setUsernameInput] = useState("");
  const [username, setUsername] = useState("");
  const [message, setMessage] = useState("");
  const [gameId, setGameId] = useState("");
  const [error, setError] = useState<string | null>(null);

  const { messages, onlineUsers, sendMessage, chatEndRef, loadOlderMessages, hasMore } =
    useChat(username, gameId);

  // 🔁 Polling funkcija za matchmaking status
  const startPolling = (playerId: string) => {
    console.log("🔁 Pokrećem polling za matchmaking status za:", playerId);
    const interval = setInterval(async () => {
      try {
        const res = await fetch(`http://localhost:5002/api/matchmaking/status/${playerId}`);
        if (!res.ok) {
          console.warn("⚠️ Neuspešan matchmaking status fetch:", res.status);
          return;
        }

        const data = await res.json();
        console.log("📡 Matchmaking status:", data);

        if (data.status === "matched" && data.matchId) {
          console.log("✅ Match pronađen! Match ID:", data.matchId);
          setGameId(data.matchId);
          clearInterval(interval);
        } else if (data.status === "searching") {
          console.log("⏳ Igrač još uvek traži protivnika...");
        } else {
          console.log("🟡 Status matchmaking-a:", data.status);
        }
      } catch (err) {
        console.error("❌ Greška tokom polling-a:", err);
        setError("Greška tokom povezivanja na matchmaking servis");
        clearInterval(interval);
      }
    }, 2000);

    return () => clearInterval(interval);
  };

  // 🔘 Kada se korisnik pridruži matchmaking-u
  const handleJoin = async () => {
    if (!usernameInput.trim()) return;
    const trimmedUsername = usernameInput.trim();
    setUsername(trimmedUsername);
    setError(null);
    console.log("👤 Korisnik se pridružuje matchmaking-u:", trimmedUsername);

    try {
      const res = await fetch("http://localhost:5002/api/matchmaking/join", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          playerId: trimmedUsername,
          rating: 1000,
        }),
      });

      const data = await res.json();
      console.log("📨 Odgovor matchmaking join:", data);

      if (!res.ok) throw new Error(data.message || "Greška pri pridruživanju matchmaking-u");

      // Počni polling
      startPolling(trimmedUsername);
    } catch (err: any) {
      console.error("❌ Greška u handleJoin:", err);
      setError(err.message);
    }
  };

  const handleSend = () => {
    if (message.trim()) {
      console.log("✉️ Šaljem poruku:", message);
      sendMessage(message);
      setMessage("");
    }
  };

  if (!username) {
    return (
      <Space direction="vertical" size="middle" style={{ width: "100%", padding: 20 }}>
        <Card title="Pridruži se Chat Sobi" style={{ maxWidth: 400, margin: "auto" }}>
          <Input
            placeholder="Unesi ime"
            value={usernameInput}
            onChange={(e) => setUsernameInput(e.target.value)}
            onPressEnter={handleJoin}
            style={{ marginBottom: 10 }}
          />
          <Button type="primary" block onClick={handleJoin}>
            Pridruži se
          </Button>
        </Card>
      </Space>
    );
  }

  return (
    <div style={{ display: "flex", height: "100vh", padding: 20, gap: 20 }}>
      <Card
        title="Online korisnici"
        style={{ width: 250, flexShrink: 0, overflowY: "auto", height: "100%" }}
        bodyStyle={{ padding: 10 }}
      >
        <OnlineUsers users={onlineUsers} />
      </Card>

      <div style={{ flex: 1, display: "flex", flexDirection: "column", maxWidth: 800 }}>
        <Title level={3}>Chat soba: {gameId ? gameId : "Traži se igra..."}</Title>
        {error && <Alert message={error} type="error" style={{ marginBottom: 10 }} />}

        <Card style={{ flex: 1, display: "flex", flexDirection: "column", padding: 10 }}>
          {hasMore && (
            <Button onClick={loadOlderMessages} style={{ marginBottom: 10 }}>
              Učitaj starije poruke
            </Button>
          )}

          <div
            style={{
              flex: 1,
              overflowY: "auto",
              display: "flex",
              flexDirection: "column",
              gap: 8,
              paddingRight: 10,
            }}
          >
            {messages.map((m, idx) => {
              const isOwn = m.sender === username;
              return (
                <div
                  key={idx}
                  style={{
                    display: "flex",
                    justifyContent: isOwn ? "flex-end" : "flex-start",
                  }}
                >
                  <div
                    style={{
                      backgroundColor: isOwn ? "#1976d2" : "#e0e0e0",
                      color: isOwn ? "white" : "black",
                      padding: "8px 12px",
                      borderRadius: 10,
                      maxWidth: "70%",
                    }}
                  >
                    <b>{m.sender}</b>
                    <Divider style={{ margin: "4px 0" }} />
                    <span>{m.content}</span>
                    <div style={{ fontSize: 10, color: "#888", textAlign: "right" }}>
                      {new Date(m.sentAt).toLocaleTimeString()}
                    </div>
                  </div>
                </div>
              );
            })}
            <div ref={chatEndRef} />
          </div>

          <div style={{ marginTop: 10, display: "flex", gap: 8 }}>
            <Input
              placeholder="Unesi poruku..."
              value={message}
              onChange={(e) => setMessage(e.target.value)}
              onPressEnter={handleSend}
              disabled={!gameId}
            />
            <Button type="primary" onClick={handleSend} disabled={!gameId}>
              Pošalji
            </Button>
          </div>
        </Card>
      </div>
    </div>
  );
}
