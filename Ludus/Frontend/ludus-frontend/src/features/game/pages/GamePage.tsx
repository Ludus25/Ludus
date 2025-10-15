import { useState, useEffect } from 'react';
import { useGame } from '../model/useGame';
import { useSearchParams, useParams, useNavigate } from 'react-router-dom';
import Board from '../ui/Board';
import { isInProgress, statusText } from '../model/types';
import { Card, Typography, Space, Alert, Button } from 'antd';
import ChatWidget from '../../chat/ui/ChatWidget';

const { Title } = Typography;
const empty9 = Array(9).fill(0) as any;

export default function GamePage() {
  const { state, loading, error, load, move } = useGame();
  const [searchParams] = useSearchParams();
  const { gameId } = useParams<{ gameId: string }>();
  const navigate = useNavigate();

  const player1FromUrl = searchParams.get('player1');
  const player2FromUrl = searchParams.get('player2');

  const [actingUserId, setActing] = useState('');
  const [x, setX] = useState(player1FromUrl || 'p1');
  const [o, setO] = useState(player2FromUrl || 'p2');
  const [localError, setLocalError] = useState<string | null>(null);

  useEffect(() => {
    if (gameId) load(gameId);
  }, [gameId]);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        setActing(payload.sub);
        if (player1FromUrl) setX(player1FromUrl);
        if (player2FromUrl) setO(player2FromUrl);
      } catch (e) {
        console.error('Failed to decode token', e);
      }
    }
  }, [player1FromUrl, player2FromUrl]);

  const handleMove = async (i: number) => {
    if (!state) return;

    const mySymbol = actingUserId === state.playerXId ? 'X' : 'O';
    if (state.nextPlayer !== mySymbol) {
      setLocalError("It's not your turn!");
      setTimeout(() => setLocalError(null), 1500);
      return;
    }

    await move(actingUserId, i);
  };

  const handleBack = () => navigate('/dashboard');

  const finished = state && !isInProgress(state.status);

  return (
    <Space direction="vertical" size="large" style={{ width: '100%' }}>
      <Title level={2}>XO Game</Title>
      <Card>
        <Space direction="vertical" style={{ width: '100%' }}>
          <Board
            cells={state?.cells ?? empty9}
            disabled={!state || !isInProgress(state.status) || loading}
            onClickCell={handleMove}
          />

          {localError && (
            <Alert
              type="warning"
              message={localError}
              showIcon
              style={{ transition: 'opacity 0.3s ease' }}
            />
          )}

          {error && <Alert type="error" message={String(error)} showIcon />}

          {finished && (
            <div style={{ textAlign: 'center', marginTop: 24 }}>
              <Title level={3}>{statusText(state.status)}</Title>
              <Button type="primary" onClick={handleBack}>
                Back to Dashboard
              </Button>
            </div>
          )}
        </Space>
      </Card>
      
      {gameId && actingUserId && (
        <ChatWidget username={actingUserId} gameId={gameId} />
      )}
    </Space>
  );
}
