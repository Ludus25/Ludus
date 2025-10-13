import { useState, useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";

export const useChat = (username: string) => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [messages, setMessages] = useState<any[]>([]);
  const [onlineUsers, setOnlineUsers] = useState<string[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [hasMore, setHasMore] = useState(true);
  const chatEndRef = useRef<HTMLDivElement>(null);

  const HUB_URL = "http://localhost:5000/chathub"; // ChatService iz VS-a

  useEffect(() => {
    if (!username) return;

    const connect = new signalR.HubConnectionBuilder()
      .withUrl(`${HUB_URL}?user=${encodeURIComponent(username)}`, {
        skipNegotiation: false,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .withAutomaticReconnect()
      .build();

    // Nova poruka
    connect.on("ReceiveMessage", (msg) => {
      setMessages((prev) =>
        [...prev, msg].sort(
          (a, b) => new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime()
        )
      );
    });

    // Online korisnici
    connect.on("UpdateUserList", (users: string[]) => setOnlineUsers(users));

    // Starije poruke
    connect.on("LoadOlderMessages", (msgs) => {
      if (msgs.length === 0) {
        setHasMore(false);
        return;
      }
      setMessages((prev) =>
        [...msgs, ...prev].sort(
          (a, b) => new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime()
        )
      );
    });

    connect.on("NoMoreMessages", () => setHasMore(false));
    connect.on("NoMatchFound", () => setError("Nije pronađena igra, pokušaj kasnije."));

    connect
      .start()
      .then(() => {
        console.log("[useChat] Connected to SignalR");
        setConnection(connect);
      })
      .catch((err) => {
        console.error("[useChat] SignalR connection error:", err);
        setError("Ne mogu da se povežem na chat: " + err.toString());
      });

    return () => connect.stop();
  }, [username]);

  const sendMessage = async (content: string) => {
    if (!connection || !content.trim()) return;
    try {
      await connection.invoke("SendMessageToGame", content.trim());
    } catch (err) {
      setError("Greška pri slanju poruke: " + err);
    }
  };

  const loadOlderMessages = async () => {
    if (!connection || !hasMore) return;

    const earliestTimestamp =
      messages.length > 0 ? new Date(messages[0].sentAt).toISOString() : null;

    try {
      await connection.invoke("LoadOlderMessages", 20, earliestTimestamp);
    } catch (err) {
      setError("Greška pri učitavanju starijih poruka: " + err);
    }
  };

  useEffect(() => {
    chatEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  return { messages, onlineUsers, sendMessage, error, chatEndRef, loadOlderMessages, hasMore };
};
