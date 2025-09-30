import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ApiConfig } from '../../Shared/Config/api.config';
import { firstValueFrom } from 'rxjs/internal/firstValueFrom';
import { PatientSearchDTO } from '../../Domain/Model/patinet-model';

@Injectable({ providedIn: 'root' })
export class PatientApiService {
patients = signal<PatientSearchDTO[]>([]);
private apiUrl = ApiConfig.LoginUrl;
constructor(private http: HttpClient) {}

  async searchPatientByMobile(mobile: string): Promise<PatientSearchDTO[]> {
    const body = { mobile };
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    try {
      const res = await firstValueFrom(
        this.http.post<PatientSearchDTO[]>(`${this.apiUrl}/${ApiConfig.endpoints.searchPatient}`, body, { headers })
      );
       console.log("Backend raw response:", res);
      this.patients.set(res || []);
      return res || [];
    } catch (error) {
      console.error('Error calling search-patient API:', error);
      return [];
    }
  }
}
