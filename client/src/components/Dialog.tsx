import type { ReactNode } from 'react'

interface DialogProps {
  title: string
  onClose: () => void
  children: ReactNode
}

export function Dialog({ title, onClose, children }: DialogProps) {
  return (
    <div className="modal-overlay">
      <div className="window dialog">
        <div className="title-bar">
          <div className="title-bar-text">{title}</div>
          <div className="title-bar-controls">
            <button type="button" aria-label="Close" onClick={onClose} />
          </div>
        </div>
        <div className="window-body">{children}</div>
      </div>
    </div>
  )
}
