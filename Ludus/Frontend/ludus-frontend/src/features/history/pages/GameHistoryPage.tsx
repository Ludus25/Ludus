import React, { useMemo, useState } from 'react'
import { Card, Input, Table, Typography, Space, Button, Flex } from 'antd'
import type { ColumnsType } from 'antd/es/table'
import { fetchGamesByEmail, fetchGameByMatchId, type GameHistoryListItem, type GameHistoryFull } from '../api/historyApi'
import { tryParseMoves, cellsAfterStep, wldRatio, resultForEmail } from '../model/types'
import Board from '../../game/ui/Board'
import dayjs from 'dayjs'
import { useNavigate } from 'react-router-dom'

const { Title, Text } = Typography

export default function GameHistoryPage() {
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [loading, setLoading] = useState(false)
  const [rows, setRows] = useState<GameHistoryListItem[]>([])
  const [selected, setSelected] = useState<GameHistoryFull | null>(null)
  const [step, setStep] = useState(0)

  const results = useMemo(() => rows.map(r => resultForEmail(r.winnerUserId ?? null, r.playerEmails, r.playerUserIds, email)), [rows, email])
  const ratio = useMemo(() => wldRatio(results), [results])

  const columns: ColumnsType<GameHistoryListItem> = [
    { title: 'Match', dataIndex: 'matchId' },
    { title: 'Opponent', render: (_, r) => {
        const emails = r.playerEmails || []
        const idx = emails.map(e => (e||'').toLowerCase()).indexOf(email.toLowerCase())
        const opp = idx !== -1 ? (emails[1-idx] || '—') : (r.playerUserIds.find(id => id !== (r.playerUserIds[idx]||'')) ?? '—')
        return <Text code>{opp}</Text>
      } 
    },
    { title: 'Started', render: (_, r) => dayjs(r.startedAt).format('YYYY-MM-DD HH:mm') },
    { title: 'Result', render: (_, r) => resultForEmail(r.winnerUserId ?? null, r.playerEmails, r.playerUserIds, email) },
  ]

  const doSearch = async () => {
    setLoading(true)
    try {
      const data = await fetchGamesByEmail(email, 1000, 0)
      setRows(data)
      setSelected(null)
      setStep(0)
    } finally {
      setLoading(false)
    }
  }

  const onPick = async (matchId: string) => {
    setLoading(true)
    try {
      const game = await fetchGameByMatchId(matchId)
      setSelected(game)
      setStep(0)
    } finally {
      setLoading(false)
    }
  }

  const moves = useMemo(() => tryParseMoves(selected?.moveHistory ?? ''), [selected])
  const cells = useMemo(() => cellsAfterStep(moves, step), [moves, step])

  return (
    <div style={{ maxWidth: 1100, margin: '24px auto', padding: 12 }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
        <Button onClick={() => navigate(-1)}>Back</Button>
        <Title level={2} style={{ margin: 0 }}>Game History</Title>
      </div>

      <Card style={{ marginBottom: 16 }}>
        <Space.Compact style={{ width: '100%' }}>
          <Input
            placeholder="Enter email"
            value={email}
            onChange={e => setEmail(e.target.value)}
            onPressEnter={doSearch}
          />
          <Button type="primary" onClick={doSearch} loading={loading}>Search</Button>
        </Space.Compact>
        <div style={{ marginTop: 12 }}>
          <Text>Total: <b>{ratio.total}</b></Text>
          <Text style={{ marginLeft: 16 }}>W/L/D: <b>{ratio.w}/{ratio.l}/{ratio.d}</b></Text>
        </div>
      </Card>

      <Card>
        <Table
          rowKey="matchId"
          loading={loading}
          dataSource={rows}
          columns={columns}
          pagination={{ pageSize: 10 }}
          onRow={(r) => ({
            onClick: () => onPick(r.matchId),
            style: { cursor: 'pointer' }
          })}
        />
      </Card>

      {selected && (
        <Card style={{ marginTop: 16 }}>
          <Title level={4}>Match {selected.matchId}</Title>
          <Text>Moves: {moves.length}</Text>
          <div style={{ margin: '16px 0' }}>
            <Board cells={cells} onClickCell={() => {}} disabled />
          </div>
          <Flex gap={8} align="center">
            <Button onClick={() => setStep(0)} disabled={step === 0}>Start</Button>
            <Button onClick={() => setStep(s => Math.max(0, s - 1))} disabled={step === 0}>Prev</Button>
            <Button onClick={() => setStep(s => Math.min(moves.length, s + 1))} disabled={step === moves.length}>Next</Button>
            <Button onClick={() => setStep(moves.length)} disabled={step === moves.length}>End</Button>
            <Text style={{ marginLeft: 12 }}>Step {step} / {moves.length}</Text>
          </Flex>
        </Card>
      )}
    </div>
  )
}
