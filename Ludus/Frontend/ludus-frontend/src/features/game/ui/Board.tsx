import { Button } from 'antd'
import { type CellEnum, renderCell } from '../model/types'

export default function Board({
  cells, disabled, onClickCell
}: { cells: CellEnum[]; disabled?: boolean; onClickCell: (i:number)=>void }) {
  return (
    <div style={{display:'flex',justifyContent:'center'}}>
      <div style={{ display:'grid', gridTemplateColumns:'repeat(3, 96px)', gap:12 }}>
        {cells.map((c, i) => (
          <Button
            key={i}
            type={c === 0 ? 'default' : 'primary'}
            size="large"
            shape="round"
            onClick={() => onClickCell(i)}
            disabled={disabled || c !== 0}
            style={{ height: 96, fontSize: 28, fontWeight: 700 }}
          >
            {renderCell(c)}
          </Button>
        ))}
      </div>
    </div>
  )
}
