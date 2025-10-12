import { Alert, Spin, Typography } from 'antd'
import type { MatchStatus } from '../model/types'

const { Text } = Typography

interface Props {
  status: MatchStatus | null
  polling: boolean
}

export default function MatchStatus({ status, polling }: Props) {
  if (!status) {
    return <Text type="secondary">Niste u matchmaking-u</Text>
  }

  if (status.status === 'searching') {
    return (
      <Alert
        type="info"
        message={
          <>
            <Spin size="small" style={{ marginRight: 8 }} />
            Tražimo protivnika...
          </>
        }
        showIcon={false}
      />
    )
  }

  if (status.status === 'matched') {
    return (
      <Alert
        type="success"
        message={`Match pronađen! MatchId: ${status.matchId}`}
        description={`Igrači: ${status.players?.join(', ')}`}
        showIcon
      />
    )
  }

  return <Text type="secondary">Status: {status.status}</Text>
}