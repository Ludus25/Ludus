import { useState, useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";

export const useChat = (username: string, gameId: string) => {
  const HUB_URL = "http://localhost:5000/chathub";

  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [messages, setMessages] = useState<any[]>([]);
  const [onlineUsers, setOnlineUsers] = useState<string[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [hasMore, setHasMore] = useState(true);
  const chatEndRef = useRef<HTMLDivElement>(null);

  // Start SignalR konekciju **tek kada postoji gameId**
  useEffect(() => {
    if (!username || !gameId) return;

    console.log("[useChat] Pokušavam da se povežem:", username, gameId);

    const connect = new signalR.HubConnectionBuilder()
      .withUrl(`${HUB_URL}?user=${encodeURIComponent(username)}&gameId=${encodeURIComponent(gameId)}`, {
        skipNegotiation: false,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .withAutomaticReconnect()
      .build();

    connect.on("ReceiveMessage", (msg) => {
      console.log("[useChat] Primljena poruka:", msg);
      setMessages((prev) =>
        [...prev, msg].sort(
          (a, b) => new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime()
        )
      );
    });

    connect.on("UpdateUserList", (users: string[]) => {
      console.log("[useChat] Online korisnici:", users);
      setOnlineUsers(users);
    });

    connect.on("LoadOlderMessages", (msgs) => {
      console.log("[useChat] LoadOlderMessages:", msgs);
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

    connect.on("NoMoreMessages", () => {
      console.log("[useChat] Nema više poruka");
      setHasMore(false);
    });

    connect.on("NoMatchFound", () => {
      console.log("[useChat] Nije pronađena igra");
      setError("Nije pronađena igra, pokušaj kasnije.");
    });

    connect
      .start()
      .then(() => {
        console.log("[useChat] Povezano!");
        setConnection(connect);
      })
      .catch((err) => {
        console.error("[useChat] Greška pri povezivanju:", err);
        setError("Greška pri povezivanju: " + err);
      });

    return () => {
      console.log("[useChat] Zaustavljam konekciju");
      connect.stop();
    };
  }, [username, gameId]);

  const sendMessage = async (content: string) => {
    if (!connection || !content.trim()) return;
    console.log("[useChat] Šaljem poruku:", content);
    try {
      await connection.invoke("SendMessageToGame", content.trim());
    } catch (err) {
      console.error("[useChat] Greška pri slanju poruke:", err);
      setError("Greška pri slanju poruke: " + err);
    }
  };

  const loadOlderMessages = async () => {
    if (!connection || !hasMore) return;
    const earliestTimestamp =
      messages.length > 0 ? new Date(messages[0].sentAt).toISOString() : null;
    console.log("[useChat] Učitavam starije poruke, od:", earliestTimestamp);
    try {
      await connection.invoke("LoadOlderMessages", 20, earliestTimestamp);
    } catch (err) {
      console.error("[useChat] Greška pri učitavanju starijih poruka:", err);
      setError("Greška pri učitavanju starijih poruka: " + err);
    }
  };

  // Scroll na kraj kada stignu nove poruke
  useEffect(() => {
    chatEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  return { messages, onlineUsers, sendMessage, error, chatEndRef, loadOlderMessages, hasMore };
};
