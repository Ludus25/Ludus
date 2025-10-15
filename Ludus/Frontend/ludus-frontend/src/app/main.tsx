import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App'
import 'antd/dist/reset.css'
import { ConfigProvider, App as AntApp } from 'antd'

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <ConfigProvider theme={{ token: { borderRadius: 12 } }}>
      <AntApp> {/* daje message/notification context */}
        <App />
      </AntApp>
    </ConfigProvider>
  </React.StrictMode>
)
