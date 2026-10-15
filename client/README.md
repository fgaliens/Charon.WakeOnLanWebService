# Charon — frontend

React + TypeScript (Vite) UI for the Wake-on-LAN backend, styled like
Windows 98 via [98.css](https://jdan.github.io/98.css/).

Features: device list with ping status refreshed every 15 seconds, adding
devices, Wake-on-LAN, editing title/IP, deleting with confirmation.

## Run

```bash
npm install
npm run dev
```

Open `http://localhost:5173`. The backend must be running on
`http://localhost:5042` and allow CORS for `http://localhost:5173` (already
configured in the backend's `appsettings.Development.json`).

The backend URL is set in `.env.development` (`VITE_API_BASE_URL`). For a
production build, create `.env.production` with the deployed backend URL
before `npm run build` — Vite bakes these variables in at build time.

## Structure

```
src/
  types.ts        — TS interfaces mirroring backend DTOs
  api.ts          — typed fetch wrappers for /devices/*
  App.tsx         — app state, main window, status bar
  components/
    Dialog.tsx               — Win98-style modal window
    DeviceTable.tsx          — device table
    ActionToolbar.tsx        — Add / Wake / Edit / Delete buttons
    AddDeviceModal.tsx       — add device form
    EditDeviceModal.tsx      — edit title / IP form
    DeleteConfirmDialog.tsx  — delete confirmation
```
