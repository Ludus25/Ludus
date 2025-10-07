import GamePage from '../features/game/pages/GamePage'

export default function App() {
  return (
    <div style={{ maxWidth: 960, margin: '24px auto', fontFamily:'system-ui, sans-serif' }}>
      <h1>Ludus Frontend</h1>
      <GamePage />
    </div>
  )
}
