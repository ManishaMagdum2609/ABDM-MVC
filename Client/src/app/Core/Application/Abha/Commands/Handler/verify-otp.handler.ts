import { AbhaLoginApiService } from "../../../../Infrastructure/Services/abhaLogin-api.service";
import { VerifyOtpCommand } from "../verify-otp.command";


export class VerifyOtpHandler {
  constructor(private apiService: AbhaLoginApiService) {}

  async execute(command: VerifyOtpCommand) {
    return this.apiService.VerifyAbhaLogin(command.txnId, command.otp, command.mobile);
  }
}

