import { useNavigate } from 'react-router-dom'
import { Button, Space, Typography } from 'antd'
import { PlayCircleOutlined, HistoryOutlined } from '@ant-design/icons'

const { Title, Text } = Typography

export default function DashboardPage() {
  const navigate = useNavigate()

  return (
    <div 
      style={{ 
        minHeight: '100vh',
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
        padding: '40px'
      }}
    >
      <Space direction="vertical" size="large" align="center" style={{ width: '100%', maxWidth: 400 }}>
        {/* Title */}
        <Title 
          level={1} 
          style={{ 
            color: 'white', 
            fontSize: '64px', 
            margin: 0,
            textShadow: '0 4px 20px rgba(0,0,0,0.3)'
          }}
        >
          🎮 Ludus
        </Title>

        <Text 
          style={{ 
            color: 'rgba(255,255,255,0.9)', 
            fontSize: '18px',
            marginBottom: '20px'
          }}
        >
          Ready to play?
        </Text>

        {/* Main Button - Start Game */}
        <Button
          type="primary"
          size="large"
          icon={<PlayCircleOutlined />}
          onClick={() => navigate('/matchmaking')}
          style={{
            width: '100%',
            height: '70px',
            fontSize: '24px',
            fontWeight: 'bold',
            borderRadius: '16px',
            background: 'white',
            color: '#667eea',
            border: 'none',
            boxShadow: '0 8px 30px rgba(0,0,0,0.3)'
          }}
        >
          Start Game
        </Button>

        {/* Secondary Button - History */}
        <Button
          size="large"
          icon={<HistoryOutlined />}
          onClick={() => navigate('/history')}
          style={{
            width: '100%',
            height: '50px',
            background: 'rgba(255,255,255,0.2)',
            color: 'white',
            border: '1px solid rgba(255,255,255,0.3)',
            borderRadius: '12px'
          }}
        >
          Game History
        </Button>
      </Space>
    </div>
  )
}