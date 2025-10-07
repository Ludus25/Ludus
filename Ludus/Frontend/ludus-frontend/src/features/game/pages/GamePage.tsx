import { useState } from 'react';
import { useGame } from '../model/useGame';
import Board from '../ui/Board';
import { isInProgress, statusText } from '../model/types';

const empty9 = Array(9).fill(0) as any;

export default function GamePage() {
  const { state, loading, error, create, load, move } = useGame();
  const [actingUserId, setActing] = useState('p1');
  const [x, setX] = useState('p1');
  const [o, setO] = useState('p2');
  const [gid, setGid] = useState('');

  return (
    <div style={{ display: 'grid', gap: 12 }}>
      <h2>XO</h2>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12 }}>
        <div>
          <h3>Kreiraj</h3>
          <input
            value={x}
            onChange={(e) => setX(e.target.value)}
            placeholder="Player X Id"
          />
          <input
            value={o}
            onChange={(e) => setO(e.target.value)}
            placeholder="Player O Id"
          />
          <button
            onClick={() => create({ playerXId: x, playerOId: o })}
            disabled={loading}
          >
            Kreiraj
          </button>
        </div>
        <div>
          <h3>Join</h3>
          <input
            value={gid}
            onChange={(e) => setGid(e.target.value)}
            placeholder="Game Id"
          />
          <button onClick={() => load(gid)} disabled={loading}>
            Join
          </button>
        </div>
      </div>

      <div>
        <label>X-UserId: </label>
        <input
          value={actingUserId}
          onChange={(e) => setActing(e.target.value)}
        />
        <small style={{ marginLeft: 8, opacity: 0.7 }}>
          Prvi potez igra player X (obično {x}). Postavi X-UserId = {x} za prvi
          klik.
        </small>
      </div>

      <div>
        <p>
          {state
            ? isInProgress(state.status)
              ? `Na potezu: ${state.nextPlayer}`
              : statusText(state.status)
            : 'Nema aktivne igre'}
        </p>

        <Board
          cells={state?.cells ?? empty9}
          disabled={!state || !isInProgress(state.status) || loading}
          onClickCell={(i) => move(actingUserId, i)}
        />

        {state && (
          <small>
            GameId: {state.gameId} · Verzija: {state.version}
          </small>
        )}
      </div>

      {error && <p style={{ color: 'crimson' }}>{error}</p>}
    </div>
  );
}
