import * as signalR from '@microsoft/signalr'
import { APP_CONFIG } from '../../app/config'

export function buildHub(hubPath: string) {
  const url = `${APP_CONFIG.wsRoot}${hubPath}`
  return new signalR.HubConnectionBuilder()
    .withUrl(url, { transport: signalR.HttpTransportType.WebSockets })
    .withAutomaticReconnect()
    .build()
}
