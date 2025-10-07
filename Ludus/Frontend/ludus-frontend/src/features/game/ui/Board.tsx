import { type CellEnum, renderCell } from '../model/types'


export default function Board({
  cells, disabled, onClickCell
}: { cells: CellEnum[]; disabled?: boolean; onClickCell: (i:number)=>void }) {
  return (
    <div style={{ display:'grid', gridTemplateColumns:'repeat(3, 100px)', gap:8 }}>
      {cells.map((c, i) => (
        <button
          key={i}
          onClick={() => onClickCell(i)}
          disabled={disabled || c !== 0}
          style={{ fontSize:32, fontWeight:700, border:'2px solid #999', borderRadius:12 }}
        >
          {renderCell(c)}
        </button>
      ))}
    </div>
  )
}
