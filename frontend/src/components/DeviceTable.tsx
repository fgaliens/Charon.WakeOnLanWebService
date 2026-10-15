import type { DeviceResponse, PingResponse } from '../types'

interface DeviceTableProps {
  devices: DeviceResponse[]
  pings: Record<string, PingResponse>
  selectedMac: string | null
  onSelectRow: (mac: string) => void
}

export function DeviceTable({ devices, pings, selectedMac, onSelectRow }: DeviceTableProps) {
  return (
    <div className="sunken-panel table-panel">
      <table className="interactive">
        <thead>
          <tr>
            <th>Title</th>
            <th>MAC address</th>
            <th>IP address</th>
            <th>Status</th>
          </tr>
        </thead>
        <tbody>
          {devices.map((device) => {
            const ping = pings[device.macAddress]
            return (
              <tr
                key={device.macAddress}
                className={device.macAddress === selectedMac ? 'highlighted' : undefined}
                onClick={() => onSelectRow(device.macAddress)}
              >
                <td>{device.title}</td>
                <td>{device.macAddress}</td>
                <td>{device.ipAddress ?? '—'}</td>
                <td>{ping ? describePing(ping) : '—'}</td>
              </tr>
            )
          })}
        </tbody>
      </table>
    </div>
  )
}

function describePing(ping: PingResponse): string {
  if (ping.description === 'IpIsNotSetForThisDevice') {
    return '—'
  }
  return ping.success ? `✅ Active (ping ${ping.roundTripTimeMs} ms.)` : `❌ Inactive (${ping.description})`
}
