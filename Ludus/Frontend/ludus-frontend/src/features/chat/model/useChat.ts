import { useState, useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";

export const useChat = (username: string, gameId: string) => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [messages, setMessages] = useState<any[]>([]);
  const [onlineUsers, setOnlineUsers] = useState<string[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [hasMore, setHasMore] = useState(true); // za Load Older
  const chatEndRef = useRef<HTMLDivElement>(null);

  const HUB_URL = "http://localhost:5000/chathub";

  useEffect(() => {
    if (!username || !gameId) return; // ne startujemo dok nema podataka

    const connect = new signalR.HubConnectionBuilder()
      .withUrl(`${HUB_URL}?user=${encodeURIComponent(username)}&gameId=${encodeURIComponent(gameId)}`, {
        skipNegotiation: false,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .withAutomaticReconnect()
      .build();

    // Nova poruka
    connect.on("ReceiveMessage", (msg) => {
      setMessages((prev) =>
        [...prev, msg].sort((a, b) => new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime())
      );
    });

    // Online korisnici
    connect.on("UpdateUserList", (users: string[]) => {
      setOnlineUsers(users);
    });

    // Starije poruke
    connect.on("LoadOlderMessages", (msgs) => {
      if (msgs.length === 0) {
        setHasMore(false);
        return;
      }
      setMessages((prev) =>
        [...msgs, ...prev].sort((a, b) => new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime())
      );
    });

    connect.on("NoMoreMessages", () => setHasMore(false));

    connect
      .start()
      .then(() => setConnection(connect))
      .catch((err) => setError("Ne mogu da se povežem na chat: " + err.toString()));

    return () => connect.stop();
  }, [username, gameId]);

  const sendMessage = async (content: string) => {
    if (!connection || !content.trim()) return;
    try {
      await connection.invoke("SendMessageToGame", gameId, username, content.trim());
    } catch (err) {
      setError("Greška pri slanju poruke: " + err);
    }
  };

  const loadOlderMessages = async () => {
    if (!connection || !hasMore) return;

    const earliestTimestamp = messages.length > 0 ? new Date(messages[0].sentAt).toISOString() : null;

    try {
      await connection.invoke("LoadOlderMessages", gameId, 20, earliestTimestamp);
    } catch (err) {
      setError("Greška pri učitavanju starijih poruka: " + err);
    }
  };

  useEffect(() => {
    chatEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  return { messages, onlineUsers, sendMessage, error, chatEndRef, loadOlderMessages, hasMore };
};
