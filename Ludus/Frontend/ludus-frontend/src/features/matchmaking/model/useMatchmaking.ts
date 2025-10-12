import { useCallback, useEffect, useMemo, useState } from 'react'
import type { NavigateFunction } from 'react-router-dom'
import * as signalR from '@microsoft/signalr'
import { matchmakingApi } from '../api/matchmakingApi'
import type { JoinMatchRequest, MatchStatus } from './types'

export function useMatchmaking(navigate?: NavigateFunction) {
  const [status, setStatus] = useState<MatchStatus | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const hub = useMemo(() => 
    new signalR.HubConnectionBuilder()
      .withUrl('/ws/matchmakingHub')
      .withAutomaticReconnect()
      .build()
  , [])

  useEffect(() => {
    hub.start()
      .then(() => {
        console.log('[SIGNALR] Connected')
      })
      .catch(err => {
        console.error('[SIGNALR] Connection error:', err)
      })

    hub.on('MatchFound', (data: any) => {
      console.log('[SIGNALR] Match found!', data)
      
      const newStatus = {
        status: 'matched' as const,
        matchId: data.matchId,
        players: data.players
      }
      setStatus(newStatus)
      
      // ✅ DODATO - Redirect ODMAH ovde, ne čekaj useEffect
      if (navigate && data.matchId) {
        console.log('[SIGNALR] Redirecting to game...', data.matchId)
        setTimeout(() => {
          navigate(`/game?matchId=${data.matchId}`)
        }, 2000)
      }
    })

    return () => {
      hub.stop()
    }
  }, [hub, navigate])

  const join = useCallback(async (dto: JoinMatchRequest) => {
    setError(null)
    setLoading(true)
    try {
      if (hub.state !== signalR.HubConnectionState.Connected) {
        console.log('[SIGNALR] Connecting...')
        await hub.start()
      }

      await hub.invoke('RegisterPlayer', dto.playerId)
      console.log('[SIGNALR] Registered player:', dto.playerId)

      await matchmakingApi.join(dto)
      setStatus({ status: 'searching' } as any)
    } catch (e: any) {
      console.error('[SIGNALR] Error:', e)
      setError(e?.response?.data ?? e?.message ?? 'Join failed')
    } finally {
      setLoading(false)
    }
  }, [hub])

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

  return { status, loading, error, join, checkStatus, polling: status?.status === 'searching' }
}