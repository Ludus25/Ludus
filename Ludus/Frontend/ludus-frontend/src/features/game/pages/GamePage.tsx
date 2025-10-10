import { useState } from 'react';
import { useGame } from '../model/useGame';
import Board from '../ui/Board';
import { isInProgress, statusText } from '../model/types';
import { Card, Row, Col, Input, Button, Typography, Space, Alert, Divider } from 'antd';

const { Title, Text } = Typography;
const empty9 = Array(9).fill(0) as any;

export default function GamePage() {
  const { state, loading, error, create, load, move } = useGame();
  const [actingUserId, setActing] = useState('p1');
  const [x, setX] = useState('p1');
  const [o, setO] = useState('p2');
  const [gid, setGid] = useState('');

  return (
    <Space direction="vertical" size="large" style={{ width: '100%' }}>
      <Title level={2}>XO</Title>

      <Row gutter={16}>
        <Col xs={24} md={12}>
          <Card title="Kreiraj" bordered>
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
          <Card title="Join" bordered>
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
            <Text strong>X-UserId:</Text>
            <Input style={{ maxWidth: 220 }} value={actingUserId} onChange={e=>setActing(e.target.value)} />
            <Text type="secondary">Prvi potez igra X ({x}). Stavi X-UserId = {x} za prvi klik.</Text>
          </Space>

          <Divider />

          <Space direction="vertical" size="small">
            {state ? (
              isInProgress(state.status)
                ? <Text strong>Na potezu: {state.nextPlayer}</Text>
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
            <Text type="secondary">GameId: {state.gameId} · Verzija: {state.version}</Text>
          )}

          {error && <Alert type="error" message={String(error)} showIcon />}
        </Space>
      </Card>
    </Space>
  );
}
