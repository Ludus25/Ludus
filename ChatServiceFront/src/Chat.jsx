import React, { useState, useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";
import axios from "axios";
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
  const chatEndRef = useRef(null);

  const API_URL = "https://localhost:5001/api/chat";
  const HUB_URL = "https://localhost:5001/chathub";

  useEffect(() => {
    if (!user) return;

    const connect = new signalR.HubConnectionBuilder()
      .withUrl(`${HUB_URL}?user=${encodeURIComponent(user)}`)
      .withAutomaticReconnect()
      .build();

    connect.on("ReceiveMessage", (msg) => {
      setMessages((prev) =>
        [...prev, msg].sort((a, b) => new Date(a.timestamp) - new Date(b.timestamp))
      );
    });

    connect.on("UpdateUserList", (users) => {
      setOnlineUsers(users);
    });

    connect
      .start()
      .then(() => setConnection(connect))
      .catch((err) => console.error("SignalR error:", err));

    return () => connect.stop();
  }, [user]);

  useEffect(() => {
    axios
      .get(`${API_URL}/messages`) // koristi pravi endpoint
      .then((res) =>
        setMessages(
          res.data.sort((a, b) => new Date(a.timestamp) - new Date(b.timestamp))
        )
      )
      .catch((err) => console.error("Error fetching messages:", err));
  }, []);

  useEffect(() => {
    chatEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

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
      {/* Sidebar Online Users */}
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

      {/* Chat Area */}
      <Box flex={1} display="flex" flexDirection="column">
        {/* Messages */}
        <Box
          flex={1}
          overflow="auto"
          p={2}
          sx={{
            background: "linear-gradient(to bottom, #f0f4ff, #e0e7ff)",
          }}
        >
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
                      bgcolor: isOwn ? "primary.main" : "grey.200",
                      color: isOwn ? "white" : "black",
                      borderRadius: 3,
                      borderTopRightRadius: isOwn ? 0 : 3,
                      borderTopLeftRadius: isOwn ? 3 : 0,
                      position: "relative",
                      boxShadow: 2,
                      "&::after": {
                        content: '""',
                        position: "absolute",
                        width: 0,
                        height: 0,
                        borderStyle: "solid",
                        borderWidth: isOwn
                          ? "10px 0 10px 10px"
                          : "10px 10px 10px 0",
                        borderColor: isOwn
                          ? `transparent transparent transparent ${"#3f51b5"}`
                          : `transparent ${"#e0e0e0"} transparent transparent`,
                        top: 12,
                        right: isOwn ? -10 : "auto",
                        left: isOwn ? "auto" : -10,
                      },
                    }}
                  >
                    <Typography variant="subtitle2" fontWeight="bold">
                      {m.sender}
                    </Typography>
                    <Typography>{m.text}</Typography>
                    <Typography
                      variant="caption"
                      display="block"
                      textAlign="right"
                    >
                      {new Date(m.timestamp).toLocaleTimeString()}
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
