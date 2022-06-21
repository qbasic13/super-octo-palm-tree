export interface AuthReq {
  email: string,
  password: string,
  fingerprint: string
}

export interface RegReq extends AuthReq {
  lastName?: string,
  firstName: string,
  middleName?: string,
  phone: string
}

export interface AuthRes {
  isSuccess: boolean;
  message: string;
  accessToken: string;
}
export interface User {
  email: string,
  role: string,
  access: string,
  fingerprint: string
}
