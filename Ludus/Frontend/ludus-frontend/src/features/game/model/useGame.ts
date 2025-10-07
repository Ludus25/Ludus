import { useCallback, useEffect, useMemo, useState } from 'react'
import { buildHub } from '../../../shared/lib/signalr'
import { gameApi } from '../api/gameApi'
import type { CreateGameDto, GameState } from './types'

export function useGame() {
  const [state, setState] = useState<GameState | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const hub = useMemo(() => buildHub('/gamehub'), []) // 👈 koristi Upstream iz Ocelot-a
  useEffect(() => () => { hub.stop().catch(() => {}) }, [hub])

  const attach = useCallback(() => {
    hub.off('MoveMade')
    hub.on('MoveMade', (s: GameState) => setState(s))
  }, [hub])

  const ensure = useCallback(async () => {
    if (hub.state === 'Disconnected') await hub.start()
  }, [hub])

  const join = useCallback(async (gid: string) => {
    try { await hub.invoke('JoinGame', gid) } catch {}
  }, [hub])

  const create = useCallback(async (dto: CreateGameDto) => {
    setError(null); setLoading(true)
    try {
      const s = await gameApi.create(dto)
      setState(s); attach(); await ensure(); await join(s.gameId)
    } catch (e: any) { setError(e?.response?.data ?? e?.message ?? 'Create failed') }
    finally { setLoading(false) }
  }, [attach, ensure, join])

  const load = useCallback(async (gid: string) => {
    setError(null); setLoading(true)
    try {
      const s = await gameApi.get(gid)
      setState(s); attach(); await ensure(); await join(s.gameId)
    } catch (e: any) { setError(e?.response?.data ?? e?.message ?? 'Load failed') }
    finally { setLoading(false) }
  }, [attach, ensure, join])

  const move = useCallback(async (actingUserId: string, cellIndex: number) => {
    if (!state) return
    setError(null); setLoading(true)
    try {
      const s = await gameApi.move(state.gameId, { cellIndex, version: state.version }, actingUserId)
      setState(s)
    } catch (e: any) { setError(e?.response?.data ?? e?.message ?? 'Move failed') }
    finally { setLoading(false) }
  }, [state])

  return { state, loading, error, create, load, move }
}
