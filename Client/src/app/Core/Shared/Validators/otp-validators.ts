export function isValidAadhaar(aadhaar: string): boolean {
  return aadhaar.length === 12 && /^[0-9]+$/.test(aadhaar);
}

export function isValidMobile(mobile: string): boolean {
  return mobile.length === 10 && /^[0-9]+$/.test(mobile);
}

export function isValidOtp(otp: string): boolean {
  return otp.length === 6 && /^[0-9]+$/.test(otp);
}
