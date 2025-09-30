// Core/Application/Abha/Queries/handler/search-patient.handler.ts
import { Injectable } from '@angular/core';
import { SearchPatientQuery } from '../search-patinet.query';
import { PatientApiService } from '../../../../Infrastructure/Services/patient-api.service';

@Injectable({ providedIn: 'root' })
export class SearchPatientHandler {
  constructor(private apiService: PatientApiService) {}

  async execute(query: SearchPatientQuery) {
    return this.apiService.searchPatientByMobile(query.mobile);
  }
}