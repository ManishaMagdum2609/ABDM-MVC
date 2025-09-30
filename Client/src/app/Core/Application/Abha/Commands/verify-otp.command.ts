export class VerifyOtpCommand {
  constructor(
    public readonly txnId: string,
    public readonly otp: string,
    public readonly mobile: string
  ) {}
}