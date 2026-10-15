import type {
  AddDeviceRequest,
  DeviceResponse,
  PingResponse,
  UpdateDeviceIpRequest,
  UpdateDeviceTitleRequest,
} from './types'

const BASE_URL = import.meta.env.VITE_API_BASE_URL

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const problem = await response.json().catch(() => null)
    throw new Error(problem?.detail ?? problem?.title ?? `Request failed with status ${response.status}`)
  }
  const text = await response.text()
  return (text ? JSON.parse(text) : undefined) as T
}

export function listDevices(): Promise<DeviceResponse[]> {
  return fetch(`${BASE_URL}/devices/`).then((r) => handleResponse<DeviceResponse[]>(r))
}

export function addDevice(request: AddDeviceRequest): Promise<void> {
  return fetch(`${BASE_URL}/devices/`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  }).then((r) => handleResponse<void>(r))
}

export function wakeDevice(mac: string): Promise<void> {
  return fetch(`${BASE_URL}/devices/${encodeURIComponent(mac)}/wake`, {
    method: 'POST',
  }).then((r) => handleResponse<void>(r))
}

export function pingDevice(mac: string): Promise<PingResponse> {
  return fetch(`${BASE_URL}/devices/${encodeURIComponent(mac)}/ping`, {
    method: 'POST',
  }).then((r) => handleResponse<PingResponse>(r))
}

export function updateDeviceTitle(mac: string, title: string | null): Promise<void> {
  const body: UpdateDeviceTitleRequest = { title }
  return fetch(`${BASE_URL}/devices/${encodeURIComponent(mac)}/title`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  }).then((r) => handleResponse<void>(r))
}

export function updateDeviceIp(mac: string, ipAddress: string | null): Promise<void> {
  const body: UpdateDeviceIpRequest = { ipAddress }
  return fetch(`${BASE_URL}/devices/${encodeURIComponent(mac)}/ip`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  }).then((r) => handleResponse<void>(r))
}

export function deleteDevice(mac: string): Promise<void> {
  return fetch(`${BASE_URL}/devices/${encodeURIComponent(mac)}`, {
    method: 'DELETE',
  }).then((r) => handleResponse<void>(r))
}
