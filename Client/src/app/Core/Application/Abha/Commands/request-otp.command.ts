export class RequestOtpCommand {
  constructor(
    public readonly index: number,
    public readonly txnId: string
  ) {}
}