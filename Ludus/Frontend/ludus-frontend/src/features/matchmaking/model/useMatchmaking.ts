import { useCallback, useEffect, useState } from 'react'
import { matchmakingApi } from '../api/matchmakingApi'
import type { JoinMatchRequest, MatchStatus } from './types'

export function useMatchmaking() {
  const [status, setStatus] = useState<MatchStatus | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [polling, setPolling] = useState(false)

  // Polling - pita server svakih 2 sekunde
  useEffect(() => {
    if (!polling || !status || status.status !== 'searching') return

    const interval = setInterval(async () => {
      try {
        const playerIdFromStatus = (status as any).playerId // Čuvajte playerId
        if (!playerIdFromStatus) return
        
        const newStatus = await matchmakingApi.getStatus(playerIdFromStatus)
        setStatus(newStatus)
        
        if (newStatus.status === 'matched') {
          setPolling(false)
          console.log('MATCH FOUND!', newStatus.matchId)
        }
      } catch (e) {
        console.error('Polling error:', e)
      }
    }, 2000)

    return () => clearInterval(interval)
  }, [polling, status])

  const join = useCallback(async (dto: JoinMatchRequest) => {
    setError(null)
    setLoading(true)
    try {
      await matchmakingApi.join(dto)
      
      // Nakon join-a, startuj polling
      setStatus({ status: 'searching', playerId: dto.playerId } as any)
      setPolling(true)
    } catch (e: any) {
      setError(e?.response?.data ?? e?.message ?? 'Join failed')
    } finally {
      setLoading(false)
    }
  }, [])

  const checkStatus = useCallback(async (playerId: string) => {
    setError(null)
    setLoading(true)
    try {
      const s = await matchmakingApi.getStatus(playerId)
      setStatus(s)
    } catch (e: any) {
      setError(e?.response?.data ?? e?.message ?? 'Status check failed')
    } finally {
      setLoading(false)
    }
  }, [])

  return { status, loading, error, join, checkStatus, polling }
}