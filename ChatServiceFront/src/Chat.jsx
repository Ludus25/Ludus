import React, { useState, useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";
import {
  Box,
  Button,
  Paper,
  TextField,
  Typography,
  Stack,
} from "@mui/material";

const Chat = () => {
  const [connection, setConnection] = useState(null);
  const [messages, setMessages] = useState([]);
  const [onlineUsers, setOnlineUsers] = useState([]);
  const [usernameInput, setUsernameInput] = useState("");
  const [user, setUser] = useState("");
  const [message, setMessage] = useState("");
  const [showLoadOlder, setShowLoadOlder] = useState(true);
  const chatEndRef = useRef(null);

  const HUB_URL = "http://localhost:5000/chathub";

  useEffect(() => {
    if (!user) return;

    const connect = new signalR.HubConnectionBuilder()
      .withUrl(`${HUB_URL}?user=${encodeURIComponent(user)}`, {
        skipNegotiation: false,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .withAutomaticReconnect()
      .build();

    // Receive a new message
    connect.on("ReceiveMessage", (msg) => {
      setMessages((prev) =>
        [...prev, msg].sort((a, b) => new Date(a.sentAt) - new Date(b.sentAt))
      );
    });

    // Update online users list
    connect.on("UpdateUserList", (users) => {
      setOnlineUsers(users);
    });

    // Load older messages
    connect.on("LoadOlderMessages", (msgs) => {
      if (msgs.length === 0) {
        setShowLoadOlder(false); // nestane dugme ako nema više poruka
        return;
      }

      setMessages((prev) =>
        [...msgs, ...prev].sort((a, b) => new Date(a.sentAt) - new Date(b.sentAt))
      );
    });

    connect.on("NoMoreMessages", () => {
      setShowLoadOlder(false);
    });

    connect
      .start()
      .then(() => setConnection(connect))
      .catch((err) => console.error("SignalR connection failed:", err));

    return () => connect.stop();
  }, [user]);

  const handleLogin = () => {
    if (usernameInput.trim()) setUser(usernameInput.trim());
  };

  const sendMessage = async () => {
    if (connection && user && message.trim()) {
      try {
        await connection.invoke("SendMessage", user, message.trim());
        setMessage("");
      } catch (err) {
        console.error("Error sending message:", err);
      }
    }
  };

  const loadOlderMessages = async () => {
    if (!connection) return;

    const earliestTimestamp =
      messages.length > 0 ? new Date(messages[0].sentAt).toISOString() : null;

    try {
      await connection.invoke("LoadOlderMessages", 20, earliestTimestamp);
    } catch (err) {
      console.error("Error loading older messages:", err);
    }
  };

  useEffect(() => {
    chatEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  if (!user) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="100vh"
        sx={{ bgcolor: "linear-gradient(135deg, #e0e7ff, #f0f4ff)" }}
      >
        <Paper sx={{ p: 4, minWidth: 300, textAlign: "center" }}>
          <Typography variant="h5" color="primary" mb={2}>
            Welcome to Chat
          </Typography>
          <TextField
            label="Your Name"
            fullWidth
            value={usernameInput}
            onChange={(e) => setUsernameInput(e.target.value)}
            margin="normal"
          />
          <Button
            fullWidth
            variant="contained"
            color="primary"
            sx={{ mt: 2 }}
            onClick={handleLogin}
          >
            Join Chat
          </Button>
        </Paper>
      </Box>
    );
  }

  return (
    <Box display="flex" height="100vh" sx={{ bgcolor: "grey.100" }}>
      {/* Sidebar */}
      <Box
        width={250}
        p={2}
        sx={{
          bgcolor: "white",
          boxShadow: 3,
          borderRadius: 2,
          borderRight: "1px solid #ddd",
        }}
      >
        <Typography variant="h6" mb={2} color="primary">
          Online Users
        </Typography>
        <Stack spacing={1}>
          {onlineUsers.length === 0 ? (
            <Typography color="text.secondary">No users online</Typography>
          ) : (
            onlineUsers.map((u, idx) => (
              <Paper
                key={idx}
                sx={{
                  p: 1,
                  display: "flex",
                  alignItems: "center",
                  bgcolor: "#e3f2fd",
                  borderRadius: 2,
                }}
              >
                <Box
                  sx={{
                    width: 10,
                    height: 10,
                    borderRadius: "50%",
                    bgcolor: "green",
                    mr: 1.5,
                  }}
                />
                <Typography>{u}</Typography>
              </Paper>
            ))
          )}
        </Stack>
      </Box>

      {/* Chat area */}
      <Box flex={1} display="flex" flexDirection="column">
        {/* Older messages button */}
        {showLoadOlder && (
          <Box p={1} textAlign="center">
            <Button variant="outlined" size="small" onClick={loadOlderMessages}>
              Load older messages
            </Button>
          </Box>
        )}

        {/* Messages */}
        <Box flex={1} overflow="auto" p={2}>
          <Stack spacing={2}>
            {messages.map((m, idx) => {
              const isOwn = m.sender === user;
              return (
                <Box
                  key={idx}
                  display="flex"
                  justifyContent={isOwn ? "flex-end" : "flex-start"}
                >
                  <Paper
                    sx={{
                      px: 3,
                      py: 1.5,
                      maxWidth: "70%",
                      bgcolor: isOwn ? "#1976d2" : "#e0e0e0",
                      color: isOwn ? "white" : "black",
                      borderRadius: 3,
                    }}
                  >
                    <Typography variant="subtitle2" fontWeight="bold">
                      {m.sender}
                    </Typography>
                    <Typography>{m.content}</Typography>
                    <Typography
                      variant="caption"
                      display="block"
                      textAlign="right"
                    >
                      {new Date(m.sentAt).toLocaleTimeString()}
                    </Typography>
                  </Paper>
                </Box>
              );
            })}
            <div ref={chatEndRef} />
          </Stack>
        </Box>

        {/* Input bar */}
        <Box p={2} display="flex" gap={1} sx={{ bgcolor: "white" }}>
          <TextField
            fullWidth
            placeholder="Type your message..."
            value={message}
            onChange={(e) => setMessage(e.target.value)}
            onKeyDown={(e) => e.key === "Enter" && sendMessage()}
          />
          <Button
            variant="contained"
            color="primary"
            sx={{ px: 4 }}
            onClick={sendMessage}
          >
            Send
          </Button>
        </Box>
      </Box>
    </Box>
  );
};

export default Chat;
