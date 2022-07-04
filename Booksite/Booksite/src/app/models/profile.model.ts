export interface ProfileRes {
  isSuccess: boolean;
  status: string;
  message?: string;
  profile?: Profile;
}

export interface Profile {
  id: number,
  email: string,
  phone: string,
  firstName: string,
  lastName?: string,
  middleName?: string,
  accountType?: string,
  registerDate: Date
}
