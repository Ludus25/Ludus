import { Card, Typography } from 'antd'
const { Text } = Typography

interface Props {
  users: string[]
}

export default function OnlineUsers({ users }: Props) {
  return (
    <Card title="Online Korisnici" style={{ maxWidth: 300 }}>
      {users.length === 0 ? (
        <Text>Nema korisnika online</Text>
      ) : (
        <ul>
          {users.map(u => <li key={u}>{u}</li>)}
        </ul>
      )}
    </Card>
  )
}
