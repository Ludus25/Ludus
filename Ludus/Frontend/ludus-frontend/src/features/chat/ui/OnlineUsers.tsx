import { Card, Typography } from 'antd'
const { Text } = Typography

interface Props {
  users: string[]
}

export default function OnlineUsers({ users }: Props) {
  return (
    <Card title="Online Users" style={{ maxWidth: 300 }}>
      {users.length === 0 ? (
        <Text>There is no online users</Text>
      ) : (
        <ul>
          {users.map(u => <li key={u}>{u}</li>)}
        </ul>
      )}
    </Card>
  )
}
