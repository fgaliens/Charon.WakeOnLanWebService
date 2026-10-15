export interface DeviceResponse {
  macAddress: string
  ipAddress: string | null
  title: string
}

export interface PingResponse {
  success: boolean
  description: string
  roundTripTime: string
  roundTripTimeMs: number
}

export interface AddDeviceRequest {
  macAddress: string
  ipAddress: string | null
  title: string | null
}

export interface UpdateDeviceTitleRequest {
  title: string | null
}

export interface UpdateDeviceIpRequest {
  ipAddress: string | null
}
