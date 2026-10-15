import { useState } from 'react'
import type { AddDeviceRequest } from '../types'
import { Dialog } from './Dialog'

interface AddDeviceModalProps {
  onAdd: (request: AddDeviceRequest) => void | Promise<void>
  onCancel: () => void
}

export function AddDeviceModal({ onAdd, onCancel }: AddDeviceModalProps) {
  const [macAddress, setMacAddress] = useState('')
  const [ipAddress, setIpAddress] = useState('')
  const [title, setTitle] = useState('')

  return (
    <Dialog title="Add device" onClose={onCancel}>
      <div className="field-row-stacked">
        <label htmlFor="add-mac">MAC address</label>
        <input
          id="add-mac"
          type="text"
          placeholder="00-11-22-33-44-55"
          value={macAddress}
          onChange={(e) => setMacAddress(e.target.value)}
        />
      </div>
      <div className="field-row-stacked">
        <label htmlFor="add-ip">IP address</label>
        <input
          id="add-ip"
          type="text"
          placeholder="192.168.1.10"
          value={ipAddress}
          onChange={(e) => setIpAddress(e.target.value)}
        />
      </div>
      <div className="field-row-stacked">
        <label htmlFor="add-title">Title</label>
        <input
          id="add-title"
          type="text"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
        />
      </div>

      <div className="dialog-actions">
        <button
          type="button"
          className="default"
          disabled={!macAddress.trim()}
          onClick={() =>
            onAdd({
              macAddress: macAddress.trim(),
              ipAddress: ipAddress.trim() || null,
              title: title.trim() || null,
            })
          }
        >
          Add
        </button>
        <button type="button" onClick={onCancel}>
          Cancel
        </button>
      </div>
    </Dialog>
  )
}
