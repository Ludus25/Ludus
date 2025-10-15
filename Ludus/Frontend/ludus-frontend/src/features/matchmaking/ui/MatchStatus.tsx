import { Alert, Spin, Typography } from 'antd'
import type { MatchStatus } from '../model/types'

const { Text } = Typography

interface Props {
  status: MatchStatus | null
  polling: boolean
}

export default function MatchStatus({ status, polling }: Props) {
  if (!status) {
    return <Text type="secondary">Not in matchmaking</Text>
  }

  if (status.status === 'searching') {
    return (
      <Alert
        type="info"
        message={
          <>
            <Spin size="small" style={{ marginRight: 8 }} />
            Searching for opponent...
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
        message="Match Found!"
        description={`Players: ${status.players?.join(' vs ')}`}
        showIcon
      />
    )
  }

  return <Text type="secondary">Status: {status.status}</Text>
}