import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { useMatchmaking } from '../model/useMatchmaking'
import MatchStatus from '../ui/MatchStatus'
import { Card, Space, Button, Typography, Alert } from 'antd'

const { Title } = Typography

export default function MatchmakingPage() {
  const navigate = useNavigate()
  const { status, loading, error, join, polling } = useMatchmaking(navigate)
  const [playerId, setPlayerId] = useState<string | null>(null)

  useEffect(() => {
    // Ekstraktuj email iz JWT tokena
    const token = localStorage.getItem('token')
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]))
        const email = payload.sub || payload.email
        setPlayerId(email)
      } catch (e) {
        console.error('Failed to decode token', e)
        navigate('/auth')
      }
    } else {
      navigate('/auth')
    }
  }, [navigate])

  const handleJoin = async () => {
    if (!playerId) return
    
    // AUTO join sa playerId iz tokena
    await join({ playerId, rating: 1500 })
  }

  const handleCancel = () => {
    navigate('/dashboard')
  }

  return (
    <Space direction="vertical" size="large" style={{ width: '100%' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Title level={2}>Matchmaking</Title>
        <Button onClick={handleCancel} disabled={polling}>
          ← Back to Dashboard
        </Button>
      </div>

      <Card title="Find a Match">
        <Space direction="vertical" style={{ width: '100%' }}>
          {playerId && (
            <Alert 
              type="info" 
              message={`Playing as: ${playerId}`} 
              showIcon 
            />
          )}

          {!polling ? (
            <Button
              type="primary"
              size="large"
              onClick={handleJoin}
              loading={loading}
              disabled={!playerId}
              block
            >
              🎮 Find Match
            </Button>
          ) : (
            <Alert 
              type="warning" 
              message="Searching for opponent..." 
              description="Please wait, we're finding you a match!"
              showIcon 
            />
          )}

          <MatchStatus status={status} polling={polling} />

          {error && <Alert type="error" message={String(error)} showIcon />}
        </Space>
      </Card>
    </Space>
  )
}