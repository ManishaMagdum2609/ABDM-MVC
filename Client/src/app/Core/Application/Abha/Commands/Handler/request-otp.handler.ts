

import { AbhaLoginApiService } from '../../../../Infrastructure/Services/abhaLogin-api.service';
import { RequestOtpCommand } from '../request-otp.command';


export class RequestOtpHandler {
  constructor(private apiService: AbhaLoginApiService) {}

  async execute(command: RequestOtpCommand) {
    return this.apiService.RequestOtpLogin(command.index, command.txnId);
  }
}
