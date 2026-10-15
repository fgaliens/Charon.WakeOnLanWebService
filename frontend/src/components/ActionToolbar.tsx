import type { DeviceResponse } from '../types'

interface ActionToolbarProps {
  selectedDevice: DeviceResponse | null
  onAdd: () => void
  onWake: () => void
  onEdit: () => void
  onDelete: () => void
}

export function ActionToolbar({ selectedDevice, onAdd, onWake, onEdit, onDelete }: ActionToolbarProps) {
  return (
    <div className="toolbar">
      <button type="button" onClick={onAdd}>
        Add device...
      </button>
      {selectedDevice && (
        <>
          <button type="button" onClick={onWake}>
            Wake on LAN
          </button>
          <button type="button" onClick={onEdit}>
            Edit...
          </button>
          <button type="button" onClick={onDelete}>
            Delete
          </button>
        </>
      )}
    </div>
  )
}
