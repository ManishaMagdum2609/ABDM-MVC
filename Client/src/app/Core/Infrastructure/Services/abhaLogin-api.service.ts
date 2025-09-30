import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AbhaAccount, OtpResponse, VerifyOtpResponse } from '../../Domain/Model/abha-account-model';
import { ApiConfig } from '../../Shared/Config/api.config';
import { firstValueFrom } from 'rxjs/internal/firstValueFrom';


@Injectable({ providedIn: 'root' })
export class AbhaLoginApiService {
  private apiUrl = ApiConfig.LoginUrl;

  searchResult = signal<AbhaAccount[]>([]);
  otpLoginResponse = signal<OtpResponse | null>(null);
  verifyLoginResponse = signal<VerifyOtpResponse | null>(null);

  constructor(private http: HttpClient) {}

  async SearchAbha(mobile: string): Promise<AbhaAccount[]> {
    const body = { mobile };
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    try {
      const res = await firstValueFrom(
        this.http.post<AbhaAccount[]>(`${this.apiUrl}/${ApiConfig.endpoints.searchAbha}`, body, { headers })
      );
      this.searchResult.set(res || []);
      return res;
    } catch (error) {
      console.error('Error calling search-abha API:', error);
      return [];
    }
  }

  async RequestOtpLogin(index: number, txnId: string): Promise<OtpResponse | null> {
    const body = { index, txnId };
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    try {
      const res = await firstValueFrom(
        this.http.post<OtpResponse>(`${this.apiUrl}/${ApiConfig.endpoints.requestOtp}`, body, { headers })
      );
      this.otpLoginResponse.set(res);
      return res;
    } catch (error) {
      console.error('Error calling request-otp-login API:', error);
      return null;
    }
  }

  async VerifyAbhaLogin(txnId: string, otp: string, mobile: string): Promise<VerifyOtpResponse | null> {
    const body = { txnId, otp, mobile };
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    try {
      const res = await firstValueFrom(
        this.http.post<VerifyOtpResponse>(`${this.apiUrl}/${ApiConfig.endpoints.verifyOtp}`, body, { headers })
      );
      this.verifyLoginResponse.set(res);
      return res;
    } catch (error) {
      console.error('Error calling verify-abha-login API:', error);
      return null;
    }
  }
  
}
