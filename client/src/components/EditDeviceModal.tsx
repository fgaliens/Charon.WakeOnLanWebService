import { useState } from 'react'
import type { DeviceResponse } from '../types'
import { Dialog } from './Dialog'

interface EditDeviceModalProps {
  device: DeviceResponse
  onSave: (title: string, ipAddress: string | null) => void | Promise<void>
  onCancel: () => void
}

export function EditDeviceModal({ device, onSave, onCancel }: EditDeviceModalProps) {
  const [title, setTitle] = useState(device.title)
  const [ipAddress, setIpAddress] = useState(device.ipAddress ?? '')

  return (
    <Dialog title={`Edit: ${device.title}`} onClose={onCancel}>
      <div className="field-row-stacked">
        <label htmlFor="edit-title">Title</label>
        <input
          id="edit-title"
          type="text"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
        />
      </div>
      <div className="field-row-stacked">
        <label htmlFor="edit-ip">IP address</label>
        <input
          id="edit-ip"
          type="text"
          value={ipAddress}
          onChange={(e) => setIpAddress(e.target.value)}
        />
      </div>

      <div className="dialog-actions">
        <button type="button" className="default" onClick={() => onSave(title, ipAddress || null)}>
          Save
        </button>
        <button
          type="button"
          onClick={() => {
            setTitle(device.title)
            setIpAddress(device.ipAddress ?? '')
          }}
        >
          Reset
        </button>
        <button type="button" onClick={onCancel}>
          Cancel
        </button>
      </div>
    </Dialog>
  )
}
