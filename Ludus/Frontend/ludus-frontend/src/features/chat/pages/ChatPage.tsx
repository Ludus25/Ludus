import { useState } from 'react';
import { Card, Button, Input, Typography, Space, Alert, Divider } from 'antd';
import { useChat } from '../model/useChat';

const { Title } = Typography;

type OnlineUsersProps = {
  users: string[]
};

const OnlineUsers: React.FC<OnlineUsersProps> = ({ users }) => (
  <Space direction="vertical" size="small" style={{ width: '100%' }}>
    {users.length === 0 ? (
      <div style={{ color: '#888' }}>Nema online korisnika</div>
    ) : (
      users.map((u, idx) => (
        <div
          key={idx}
          style={{
            display: 'flex',
            alignItems: 'center',
            padding: '4px 8px',
            backgroundColor: '#e3f2fd',
            borderRadius: 6,
          }}
        >
          <div
            style={{
              width: 10,
              height: 10,
              borderRadius: '50%',
              backgroundColor: 'green',
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
  const [usernameInput, setUsernameInput] = useState('');
  const [gameIdInput, setGameIdInput] = useState('');
  const [username, setUsername] = useState('');
  const [gameId, setGameId] = useState('');
  const [message, setMessage] = useState('');

  const { messages, onlineUsers, sendMessage, error, chatEndRef, loadOlderMessages, hasMore } =
    useChat(username, gameId);

  const handleJoin = () => {
    if (usernameInput.trim() && gameIdInput.trim()) {
      setUsername(usernameInput.trim());
      setGameId(gameIdInput.trim());
    }
  };

  const handleSend = () => {
    if (message.trim()) {
      sendMessage(message);
      setMessage('');
    }
  };

  if (!username || !gameId) {
    return (
      <Space direction="vertical" size="middle" style={{ width: '100%', padding: 20 }}>
        <Card title="Pridruži se Chat Sobi" style={{ maxWidth: 400, margin: 'auto' }}>
          <Input
            placeholder="Unesi ime"
            value={usernameInput}
            onChange={(e) => setUsernameInput(e.target.value)}
            onPressEnter={handleJoin}
            style={{ marginBottom: 10 }}
          />
          <Input
            placeholder="Unesi gameId"
            value={gameIdInput}
            onChange={(e) => setGameIdInput(e.target.value)}
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
    <div style={{ display: 'flex', height: '100vh', padding: 20, gap: 20 }}>
      {/* Sidebar - online users */}
      <Card
        title="Online korisnici"
        style={{ width: 250, flexShrink: 0, overflowY: 'auto', height: '100%' }}
        bodyStyle={{ padding: 10 }}
      >
        <OnlineUsers users={onlineUsers} />
      </Card>

      {/* Chat area */}
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', maxWidth: 800 }}>
        <Title level={3}>Chat soba: {gameId}</Title>
        {error && <Alert message={error} type="error" style={{ marginBottom: 10 }} />}

        <Card style={{ flex: 1, display: 'flex', flexDirection: 'column', padding: 10 }}>
          {/* Load older messages button */}
          {hasMore && (
            <Button onClick={loadOlderMessages} style={{ marginBottom: 10 }}>
              Učitaj starije poruke
            </Button>
          )}

          {/* Poruke */}
          <div
            style={{
              flex: 1,
              overflowY: 'auto',
              display: 'flex',
              flexDirection: 'column',
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
                    display: 'flex',
                    justifyContent: isOwn ? 'flex-end' : 'flex-start',
                  }}
                >
                  <div
                    style={{
                      backgroundColor: isOwn ? '#1976d2' : '#e0e0e0',
                      color: isOwn ? 'white' : 'black',
                      padding: '8px 12px',
                      borderRadius: 10,
                      maxWidth: '70%',
                    }}
                  >
                    <b>{m.sender}</b>
                    <Divider style={{ margin: '4px 0' }} />
                    <span>{m.content}</span>
                    <div style={{ fontSize: 10, color: '#888', textAlign: 'right' }}>
                      {new Date(m.sentAt).toLocaleTimeString()}
                    </div>
                  </div>
                </div>
              );
            })}
            <div ref={chatEndRef} />
          </div>

          {/* Input */}
          <div style={{ marginTop: 10, display: 'flex', gap: 8 }}>
            <Input
              placeholder="Unesi poruku..."
              value={message}
              onChange={(e) => setMessage(e.target.value)}
              onPressEnter={handleSend}
            />
            <Button type="primary" onClick={handleSend}>
              Pošalji
            </Button>
          </div>
        </Card>
      </div>
    </div>
  );
}
