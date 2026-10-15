import { useEffect, useState } from 'react'
import './App.css'
import * as api from './api'
import { ActionToolbar } from './components/ActionToolbar'
import { AddDeviceModal } from './components/AddDeviceModal'
import { DeleteConfirmDialog } from './components/DeleteConfirmDialog'
import { DeviceTable } from './components/DeviceTable'
import { EditDeviceModal } from './components/EditDeviceModal'
import type { AddDeviceRequest, DeviceResponse, PingResponse } from './types'

function App() {
  const [devices, setDevices] = useState<DeviceResponse[]>([])
  const [selectedMac, setSelectedMac] = useState<string | null>(null)
  const [pings, setPings] = useState<Record<string, PingResponse>>({})
  const [isAdding, setIsAdding] = useState(false)
  const [pendingDelete, setPendingDelete] = useState<DeviceResponse | null>(null)
  const [editingDevice, setEditingDevice] = useState<DeviceResponse | null>(null)
  const [notification, setNotification] = useState<string | null>(null)

  useEffect(() => {
    api.listDevices().then(setDevices).catch(console.error)
  }, [])

  useEffect(() => {
    pingAll()

    const id = setInterval(pingAll, 15000)
    return () => clearInterval(id)
  }, [devices])

  const selectedDevice = devices.find((d) => d.macAddress === selectedMac) ?? null

  async function pingAll() {
    const pingResults = await Promise.allSettled(
      devices.map((device) => api.pingDevice(device.macAddress))
    )

    const next: Record<string, PingResponse> = {}
    pingResults.forEach((r, i) => {
      if (r.status === 'fulfilled') {
        next[devices[i].macAddress] = r.value
      }
    })

    setPings(next)
  }

  function handleSelectRow(mac: string) {
    setSelectedMac((prev) => (prev === mac ? null : mac))
  }

  function showNotification(text: string) {
    setNotification(text)
    setTimeout(() => setNotification(null), 4000)
  }

  async function handleAdd(request: AddDeviceRequest) {
    try {
      await api.addDevice(request)
      setDevices(await api.listDevices())
      setIsAdding(false)
      showNotification('Device added')
    } catch (err) {
      showNotification(`Add failed: ${(err as Error).message}`)
    }
  }

  async function handleWake(mac: string) {
    try {
      await api.wakeDevice(mac)
      showNotification('Wake request sent successfully')
    } catch (err) {
      showNotification(`Error on sending wake request, try again. ${(err as Error).message}`)
    }
  }

  async function handleConfirmDelete() {
    if (!pendingDelete)
      return

    try {
      await api.deleteDevice(pendingDelete.macAddress)
      setDevices(await api.listDevices())
    } catch (err) {
      showNotification(`Delete failed: ${(err as Error).message}`)
    } finally {
      setPendingDelete(null)
    }
  }

  async function handleSaveEdit(title: string, ipAddress: string | null) {
    if (!editingDevice)
      return

    const patches: Promise<void>[] = []
    if (title !== editingDevice.title) {
      patches.push(api.updateDeviceTitle(editingDevice.macAddress, title))
    }

    if (ipAddress !== editingDevice.ipAddress) {
      patches.push(api.updateDeviceIp(editingDevice.macAddress, ipAddress))
    }

    try {
      await Promise.all(patches)
      setDevices(await api.listDevices())
      setEditingDevice(null)
    } catch (err) {
      showNotification(`Save failed: ${(err as Error).message}`)
    }
  }

  return (
    <div className="window app-window">
      <div className="title-bar">
        <div className="title-bar-text">Wake on LAN — Devices</div>
      </div>

      <div className="window-body">
        <ActionToolbar
          selectedDevice={selectedDevice}
          onAdd={() => setIsAdding(true)}
          onWake={() => selectedDevice && handleWake(selectedDevice.macAddress)}
          onEdit={() => setEditingDevice(selectedDevice)}
          onDelete={() => setPendingDelete(selectedDevice)}
        />

        <DeviceTable
          devices={devices}
          pings={pings}
          selectedMac={selectedMac}
          onSelectRow={handleSelectRow}
        />
      </div>

      <div className="status-bar">
        <p className="status-bar-field">{notification ?? 'Ready'}</p>
        <p className="status-bar-field status-bar-count">{devices.length} device(s)</p>
      </div>

      {isAdding && (
        <AddDeviceModal onAdd={handleAdd} onCancel={() => setIsAdding(false)} />
      )}

      {pendingDelete && (
        <DeleteConfirmDialog
          device={pendingDelete}
          onConfirm={handleConfirmDelete}
          onCancel={() => setPendingDelete(null)}
        />
      )}

      {editingDevice && (
        <EditDeviceModal
          device={editingDevice}
          onSave={handleSaveEdit}
          onCancel={() => setEditingDevice(null)}
        />
      )}
    </div>
  )
}

export default App
