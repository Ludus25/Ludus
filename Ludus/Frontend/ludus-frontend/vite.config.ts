import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'

export default defineConfig({
  plugins: [react()],
  resolve: { alias: { '@': path.resolve(__dirname, 'src') } },
  server: {
    port: 5173,
    strictPort: true,
    proxy: {
      '/api': {
        target: 'http://localhost:8010',  // Gateway
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/api/, ''),
      },
      '/ws/matchmakingHub': {
        target: 'http://localhost:5002',  // Matchmaking Service
        changeOrigin: true,
        ws: true,
        rewrite: (path) => path.replace(/^\/ws\/matchmakingHub/, '/matchmakingHub')
      },
      '/ws/gamehub': {
        target: 'http://localhost:8001',  // Game Service
        changeOrigin: true,
        ws: true,
        rewrite: (path) => path.replace(/^\/ws\/gamehub/, '/gamehub')
      }
    }
  }
})