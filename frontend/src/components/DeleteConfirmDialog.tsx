import type { DeviceResponse } from '../types'
import { Dialog } from './Dialog'

interface DeleteConfirmDialogProps {
  device: DeviceResponse
  onConfirm: () => void
  onCancel: () => void
}

export function DeleteConfirmDialog({ device, onConfirm, onCancel }: DeleteConfirmDialogProps) {
  return (
    <Dialog title="Confirm Device Delete" onClose={onCancel}>
      <p>
        Are you sure you want to delete «{device.title}» ({device.macAddress})?
      </p>
      <div className="dialog-actions">
        <button type="button" className="default" onClick={onConfirm}>
          Yes
        </button>
        <button type="button" onClick={onCancel}>
          No
        </button>
      </div>
    </Dialog>
  )
}
