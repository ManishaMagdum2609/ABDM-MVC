import { AbhaRegistrationApiService } from '../../../../Infrastructure/Services/abhaRegistration-api.service';
import { RequestRegistationOtpCommand } from '../request-registration-otp.command';


export class RequestRegistrationOtpHandler {
  constructor(private apiService: AbhaRegistrationApiService) {}

  async execute(command: RequestRegistationOtpCommand) {
    return this.apiService.RequestRegisterOtp(command.AadhaarNumber);
  }
}