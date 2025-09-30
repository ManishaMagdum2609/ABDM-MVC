import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { ApiConfig } from '../../Shared/Config/api.config';
import { firstValueFrom } from 'rxjs/internal/firstValueFrom';
import { RegisterOtpResponse, VerifyRegistrationOtpResponse } from '../../Domain/Model/abha-register-model';


@Injectable({ providedIn: 'root' })
export class AbhaRegistrationApiService {
private apiUrl = ApiConfig.RegisterUrl;
  public registerOtpResponse = signal<RegisterOtpResponse | null>(null);

  constructor(private http: HttpClient) {}

  async RequestRegisterOtp(aadhar: string): Promise<RegisterOtpResponse | null> {
    const body = { AadhaarNumber:aadhar };
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    try {
      const res = await firstValueFrom(
        this.http.post<RegisterOtpResponse>(`${this.apiUrl}/request-otp-register`, body, { headers })
      );
      this.registerOtpResponse.set(res);
      return res;
    } catch (error) {
      console.error('Error calling request-register-otp API:', error);
      return null;
    }
  }
 async VerifyAbhaRegistration(txnId: string, otp: string, mobile: string): Promise<any> {
    const body = { txnId, otp, mobile };
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    try {
      return await firstValueFrom(
        this.http.post<any>(`${this.apiUrl}/verify-otp-register`, body, { headers })
      );
    } catch (error) {
      console.error('Error verifying registration OTP:', error);
      return null;
    }
  }

}