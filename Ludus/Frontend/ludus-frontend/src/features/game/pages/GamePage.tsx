import { useState, useEffect } from 'react';
import { useGame } from '../model/useGame';
import { useSearchParams, useParams } from 'react-router-dom';
import Board from '../ui/Board';
import { isInProgress, statusText } from '../model/types';
import { Card, Row, Col, Input, Button, Typography, Space, Alert, Divider } from 'antd';

const { Title, Text } = Typography;
const empty9 = Array(9).fill(0) as any;

export default function GamePage() {
  const { state, loading, error, create, load, move } = useGame();
  const [searchParams] = useSearchParams();
  const { gameId } = useParams<{ gameId: string }>();
  
  const player1FromUrl = searchParams.get('player1');
  const player2FromUrl = searchParams.get('player2');
  
  const [actingUserId, setActing] = useState('');
  const [x, setX] = useState(player1FromUrl || 'p1');
  const [o, setO] = useState(player2FromUrl || 'p2');
  const [gid, setGid] = useState(gameId || '');

  useEffect(() => {
    if (gameId) {
      console.log('[GAME] Auto-loading game:', gameId);
      console.log('[GAME] Player1:', player1FromUrl, 'Player2:', player2FromUrl);
      load(gameId);
    }
  }, [gameId]);

  useEffect(() => {
    const token = localStorage.getItem('token')
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]))
        const myUserId = payload.sub
        
        console.log('[GAME] My userId from token:', myUserId)
        console.log('[GAME] Player1 from URL:', player1FromUrl)
        console.log('[GAME] Player2 from URL:', player2FromUrl)
        
        setActing(myUserId)
        
        if (player1FromUrl) setX(player1FromUrl)
        if (player2FromUrl) setO(player2FromUrl)
      } catch (e) {
        console.error('Failed to decode token', e)
      }
    }
  }, [player1FromUrl, player2FromUrl])

  return (
    <Space direction="vertical" size="large" style={{ width: '100%' }}>
      <Title level={2}>XO Game</Title>

      {(player1FromUrl && player2FromUrl) && (
        <Alert 
          type="info" 
          message={`Match: ${player1FromUrl} (X) vs ${player2FromUrl} (O)`} 
          showIcon 
        />
      )}

      <Row gutter={16}>
        <Col xs={24} md={12}>
          <Card title="Kreiraj Novu Igru" bordered>
            <Space direction="vertical" style={{ width: '100%' }}>
              <Input value={x} onChange={e=>setX(e.target.value)} placeholder="Player X Id" />
              <Input value={o} onChange={e=>setO(e.target.value)} placeholder="Player O Id" />
              <Button type="primary" onClick={()=>create({ playerXId: x, playerOId: o })} loading={loading}>
                Kreiraj
              </Button>
            </Space>
          </Card>
        </Col>
        <Col xs={24} md={12}>
          <Card title="Join Postojeću Igru" bordered>
            <Space direction="vertical" style={{ width: '100%' }}>
              <Input value={gid} onChange={e=>setGid(e.target.value)} placeholder="Game Id" />
              <Button onClick={()=>load(gid)} loading={loading}>
                Join
              </Button>
            </Space>
          </Card>
        </Col>
      </Row>

      <Card>
        <Space direction="vertical" style={{ width: '100%' }}>
          <Space align="center">
            <Text strong>Tvoj UserId:</Text>
            <Input 
              style={{ maxWidth: 220 }} 
              value={actingUserId} 
              onChange={e=>setActing(e.target.value)} 
              disabled
            />
            <Text type="secondary">Trenutno igraš kao: {actingUserId}</Text>
          </Space>

          <Divider />

          <Space direction="vertical" size="small">
            {state ? (
              isInProgress(state.status)
                ? <Text strong>Na potezu: {state.nextPlayer} ({state.nextPlayer === 'X' ? x : o})</Text>
                : <Alert type={state.status === 1 || state.status === 'XWon' ? 'success' :
                               state.status === 2 || state.status === 'OWon' ? 'warning' : 'info'}
                         message={statusText(state.status)} showIcon />
            ) : <Text type="secondary">Nema aktivne igre</Text>}
          </Space>

          <Board
            cells={state?.cells ?? empty9}
            disabled={!state || !isInProgress(state.status) || loading}
            onClickCell={(i)=>move(actingUserId, i)}
          />

          {state && (
            <Space direction="vertical">
              <Text type="secondary">GameId: {state.gameId}</Text>
              <Text type="secondary">Player X: {state.playerXId || x}</Text>
              <Text type="secondary">Player O: {state.playerOId || o}</Text>
              <Text type="secondary">Verzija: {state.version}</Text>
            </Space>
          )}

          {error && <Alert type="error" message={String(error)} showIcon />}
        </Space>
      </Card>
    </Space>
  );
}