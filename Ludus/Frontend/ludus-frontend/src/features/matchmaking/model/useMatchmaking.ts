import { useCallback, useEffect, useMemo, useState, useRef } from 'react'
import type { NavigateFunction } from 'react-router-dom'
import * as signalR from '@microsoft/signalr'
import { matchmakingApi } from '../api/matchmakingApi'
import type { JoinMatchRequest, MatchStatus } from './types'

export function useMatchmaking(navigate?: NavigateFunction) {
  const [status, setStatus] = useState<MatchStatus | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const isConnecting = useRef(false)  // ✅ DODATO

  const hub = useMemo(() => 
    new signalR.HubConnectionBuilder()
      .withUrl('/ws/matchmakingHub', {
        accessTokenFactory: () => localStorage.getItem('token') || ''
      })
      .withAutomaticReconnect()
      .build()
  , [])

  useEffect(() => {
    const startConnection = async () => {
      // ✅ Izbegni duplicate pokušaje
      if (isConnecting.current) return
      if (hub.state !== signalR.HubConnectionState.Disconnected) return

      isConnecting.current = true
      
      try {
        await hub.start()
        console.log('[SIGNALR] Connected')
      } catch (err) {
        console.error('[SIGNALR] Connection error:', err)
      } finally {
        isConnecting.current = false
      }
    }

    startConnection()

    hub.on('MatchFound', (data: any) => {
      console.log('[SIGNALR] Match found!', data)
      
      const newStatus = {
        status: 'matched' as const,
        matchId: data.matchId,
        gameUrl: data.gameUrl,
        players: data.players
      }
      setStatus(newStatus)
      
      if (navigate) {
        setTimeout(() => {
          if (data.gameUrl) {
            console.log('[SIGNALR] Redirecting to gameUrl:', data.gameUrl)
            const url = new URL(data.gameUrl)
            const gameId = url.pathname.split('/').pop()
            const player1 = url.searchParams.get('player1')
            const player2 = url.searchParams.get('player2')
            
            navigate(`/game/${gameId}?player1=${player1}&player2=${player2}`)
          } else if (data.matchId) {
            console.log('[SIGNALR] Fallback redirect with matchId')
            navigate(`/game?matchId=${data.matchId}`)
          }
        }, 2000)
      }
    })

    return () => {
      hub.off('MatchFound')  // ✅ Ukloni listener
      
      if (hub.state === signalR.HubConnectionState.Connected) {
        hub.stop().catch(err => console.error('[SIGNALR] Stop error:', err))
      }
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