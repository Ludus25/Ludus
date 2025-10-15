import type { CellEnum } from '../../game/model/types'

export type ParsedMove = number // 0..8

export const tryParseMoves = (raw: string): ParsedMove[] => {
  try {
    const j = JSON.parse(raw)
    if (Array.isArray(j) && j.every(x => Number.isInteger(x) && x >= 0 && x <= 8)) return j as number[]
  } catch {}
  return (raw.match(/[0-8]/g) || []).map(s => parseInt(s, 10))
}

export const cellsAfterStep = (moves: ParsedMove[], step: number): CellEnum[] => {
  const cells: CellEnum[] = Array(9).fill(0)
  for (let i = 0; i < Math.min(step, moves.length); i++) {
    const idx = moves[i]
    if (idx >= 0 && idx < 9) {
      cells[idx] = i % 2 === 0 ? 1 : 2 // X then O
    }
  }
  return cells
}

export const resultForEmail = (winnerUserId: string | null | undefined, emails: string[] | undefined, ids: string[], email: string) => {
  if (!winnerUserId) return 'D'
  const list = emails?.map(e => (e || '').toLowerCase()) ?? []
  const target = email.toLowerCase()
  const idx = list.indexOf(target)
  if (idx === -1) return '—'
  return ids[idx] === winnerUserId ? 'W' : 'L'
}

export const wldRatio = (results: ('W'|'L'|'D'|'—')[]) => {
  const w = results.filter(r => r === 'W').length
  const l = results.filter(r => r === 'L').length
  const d = results.filter(r => r === 'D').length
  return { w, l, d, total: results.length }
}
