import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useMatchmaking } from '../model/useMatchmaking'
import MatchStatus from '../ui/MatchStatus'
import { Card, Space, Input, Button, Typography, Alert, InputNumber } from 'antd'

const { Title, Text } = Typography

export default function MatchmakingPage() {
  const navigate = useNavigate()
  const { status, loading, error, join, checkStatus } = useMatchmaking(navigate)
  const [playerId, setPlayerId] = useState('player1')
  const [rating, setRating] = useState(1500)

  const handleJoin = async () => {
    await join({ playerId, rating })
  }

  return (
    <Space direction="vertical" size="large" style={{ width: '100%' }}>
      <Title level={2}>Matchmaking</Title>

      <Card title="Pridruži se Matchmaking-u" variant="outlined">
        <Space direction="vertical" style={{ width: '100%' }}>
          <div>
            <Text>Player ID:</Text>
            <Input
              value={playerId}
              onChange={e => setPlayerId(e.target.value)}
              placeholder="Unesi svoj ID"
              disabled={status?.status === 'searching'}
            />
          </div>

          <div>
            <Text>Rating:</Text>
            <InputNumber
              value={rating}
              onChange={v => setRating(v ?? 1500)}
              min={0}
              max={3000}
              style={{ width: '100%' }}
              disabled={status?.status === 'searching'}
            />
          </div>

          <Button
            type="primary"
            onClick={handleJoin}
            loading={loading}
            disabled={status?.status === 'searching'}
            block
          >
            {status?.status === 'searching' ? 'Tražimo protivnika...' : 'Nađi Match'}
          </Button>

          <MatchStatus status={status} polling={status?.status === 'searching'} />

          {error && <Alert type="error" message={String(error)} showIcon />}
        </Space>
      </Card>

      <Card title="Proveri Status" variant="outlined">
        <Space direction="vertical" style={{ width: '100%' }}>
          <Input
            placeholder="Player ID za proveru"
            onPressEnter={(e) => checkStatus((e.target as HTMLInputElement).value)}
          />
          <Button onClick={() => checkStatus(playerId)} loading={loading}>
            Proveri Status
          </Button>
        </Space>
      </Card>
    </Space>
  )
}