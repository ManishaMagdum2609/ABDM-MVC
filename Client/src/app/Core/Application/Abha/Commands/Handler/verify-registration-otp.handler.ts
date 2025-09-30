
import { AbhaRegistrationApiService } from "../../../../Infrastructure/Services/abhaRegistration-api.service";
import { VerifyRegistrationOtpCommand } from "../verify-registration-otp.command";


export class VerifyRegistrationOtpHandler {
  constructor(private apiService: AbhaRegistrationApiService) {}

  async execute(command: VerifyRegistrationOtpCommand) {
    return this.apiService.VerifyAbhaRegistration(command.txnId, command.otp, command.mobile);
  }
}