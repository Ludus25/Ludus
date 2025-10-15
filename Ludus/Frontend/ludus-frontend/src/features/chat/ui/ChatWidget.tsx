import { useState } from "react";
import { Card, Button, Input, Divider } from "antd";
import { useChat } from "../model/useChat";

export default function ChatWidget({ username, gameId }: { username: string; gameId: string }) {
  const [message, setMessage] = useState("");
  const { messages, sendMessage, chatEndRef } = useChat(username, gameId);
  const [open, setOpen] = useState(true);

  const handleSend = () => {
    if (message.trim()) {
      sendMessage(message);
      setMessage("");
    }
  };

  if (!open)
    return (
      <Button
        type="primary"
        style={{ position: "fixed", bottom: 20, right: 20, zIndex: 1000 }}
        onClick={() => setOpen(true)}
      >
        💬 Chat
      </Button>
    );

  return (
    <Card
      title={
        <div style={{ display: "flex", justifyContent: "space-between" }}>
          <span>Game Chat</span>
          <Button size="small" onClick={() => setOpen(false)}>
            ✖
          </Button>
        </div>
      }
      style={{
        position: "fixed",
        bottom: 20,
        right: 20,
        width: 300,
        height: 400,
        display: "flex",
        flexDirection: "column",
        zIndex: 1000,
      }}
      bodyStyle={{ display: "flex", flexDirection: "column", flex: 1 }}
    >
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
        {messages.map((m, i) => {
          const isOwn = m.sender === username;
          return (
            <div key={i} style={{ display: "flex", justifyContent: isOwn ? "flex-end" : "flex-start" }}>
              <div
                style={{
                  backgroundColor: isOwn ? "#1976d2" : "#e0e0e0",
                  color: isOwn ? "white" : "black",
                  padding: "6px 10px",
                  borderRadius: 10,
                  maxWidth: "70%",
                }}
              >
                <b>{m.sender}</b>
                <Divider style={{ margin: "4px 0" }} />
                <span>{m.content}</span>
              </div>
            </div>
          );
        })}
        <div ref={chatEndRef} />
      </div>

      <div style={{ marginTop: 10, display: "flex", gap: 8 }}>
        <Input
          placeholder="Type..."
          value={message}
          onChange={(e) => setMessage(e.target.value)}
          onPressEnter={handleSend}
        />
        <Button type="primary" onClick={handleSend}>
          Send
        </Button>
      </div>
    </Card>
  );
}
