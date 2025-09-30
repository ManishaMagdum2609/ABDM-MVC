export interface OtpResponse {
  txnId: string;
  message: string;
}

export interface AbhaAccountDto {
  abhaNumber: string;
  preferredAbhaAddress: string;
  name: string;
}

export interface VerifyOtpResponse {
  txnId: string;
  accounts: AbhaAccountDto[];
}

export interface AbhaEntry {
  index: number;
  abhaNumber: string;
  name: string;
  gender?: string;
  kycVerified?: string;
  authMethods?: string[];
}

export interface AbhaAccount {
  txnId: string;
  abha: AbhaEntry[];
}
