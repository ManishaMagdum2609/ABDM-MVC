
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';

import { SearchAbhaAccountQuery } from '../../../Application/Abha/Queries/search-abha-account.query';
import { RequestOtpCommand } from '../../../Application/Abha/Commands/request-otp.command';
import { VerifyOtpCommand } from '../../../Application/Abha/Commands/verify-otp.command';
import { Component, signal } from '@angular/core';
import { AbhaAccountDto } from '../../../Domain/Model/abha-account-model';
import { SearchAbhaAccountHandler } from '../../../Application/Abha/Queries/handler/search-abha-account.handler';
import { RequestOtpHandler } from '../../../Application/Abha/Commands/Handler/request-otp.handler';
import { VerifyOtpHandler } from '../../../Application/Abha/Commands/Handler/verify-otp.handler';
import { ToastrService } from 'ngx-toastr';
import { isValidOtp } from '../../../Shared/Validators/otp-validators';
import { PatientApiService } from '../../../Infrastructure/Services/patient-api.service';
import { SearchPatientHandler } from '../../../Application/Abha/Queries/handler/search-patient.handler';
import { SearchPatientQuery } from '../../../Application/Abha/Queries/search-patinet.query';
import { PatientSearchDTO } from '../../../Domain/Model/patinet-model';
import { AbhaRegistration } from '../abha-registration/abha-registration';
import { AbhaLoginApiService } from '../../../Infrastructure/Services/abhaLogin-api.service';

@Component({
  selector: 'app-abha',
  standalone: true,
  imports: [CommonModule, FormsModule,AbhaRegistration,ReactiveFormsModule],
  templateUrl: './abhaLogin-component.html',
  styleUrls: ['./abhaLogin-component.css']
})
export class ABHA {
  Step = signal(1);  //A signal in Angular is like a special variable.When its value changes,Angular automatically updates 
                     // the UI wherever that value is used.You don’t need to manually tell Angular to refresh.
  Mobile = signal('');
  Otp = signal('');
  TxnId = signal('');
  AbhaAccounts = signal<AbhaAccountDto[]>([]);
  SelectedAbha = signal<AbhaAccountDto | null>(null);
  Patients = signal<PatientSearchDTO[]>([]);
  SelectedPatient = signal<PatientSearchDTO | null>(null);
  ShowCreateAbha = signal(false);
  AbhaSearched = signal(false);    
  NoAbhaFound = signal(false); 
  Loading = signal(false);
  loginForm: FormGroup;
  private searchHandler: SearchAbhaAccountHandler;
  private otpHandler: RequestOtpHandler;
  private verifyHandler: VerifyOtpHandler;
  private searchPatientHandler:SearchPatientHandler;

  constructor(
    private fb: FormBuilder,
    private apiService: AbhaLoginApiService, 
    private toastr: ToastrService, 
    private patientService: PatientApiService,) {
      this.loginForm = this.fb.group({
      mobile: ['', [Validators.required, Validators.pattern('^[0-9]{10}$')]],
      otp: ['', [Validators.pattern('^[0-9]{6}$')]]
    });
    this.searchHandler = new SearchAbhaAccountHandler(apiService);
    this.otpHandler = new RequestOtpHandler(apiService);
    this.verifyHandler = new VerifyOtpHandler(apiService);
    this.searchPatientHandler = new SearchPatientHandler(patientService);
  }
  OnMobileInput(value: string) {
  this.loginForm.get('mobile')?.setValue(value);

  if (value.length !== 10) {
    this.Step.set(1);
    this.AbhaSearched.set(false);
    this.NoAbhaFound.set(false);
    this.AbhaAccounts.set([]);
    this.Patients.set([]);
    this.ShowCreateAbha.set(false);
  }
}

  OnCreateAbhaClick() {
  this.ShowCreateAbha.set(true);   
  this.Step.set(1);                
  this.NoAbhaFound.set(false);     
  this.Patients.set([]);           
}
async RequestOtp(): Promise<void> {
  if (this.loginForm.get('mobile')?.invalid) {
    this.toastr.error('Enter a valid 10-digit mobile number');
    return;
  }

  this.Loading.set(true);

  try {
    this.AbhaSearched.set(true);
    this.NoAbhaFound.set(false);
    this.AbhaAccounts.set([]);
    this.Patients.set([]);

    const mobile = this.loginForm.get('mobile')!.value;
    const searchResult = await this.searchHandler.execute(new SearchAbhaAccountQuery(mobile));

    if (!searchResult || searchResult.length === 0) {
      this.NoAbhaFound.set(true);
      this.Step.set(2);                         
      this.toastr.info('No ABHA account found for this mobile number.');
      await this.SearchPatientInDanphe(mobile); 
      return; 
    }

    const firstEntry = searchResult[0];
    if (!firstEntry.abha || firstEntry.abha.length === 0) {
      this.NoAbhaFound.set(true);
      this.Step.set(2);
      this.toastr.info('No ABHA entries found for this mobile number.');
      await this.SearchPatientInDanphe(mobile);
      return;
    }

    this.NoAbhaFound.set(false);
    const accountIndex = firstEntry.abha[0].index;
    const otpResponse = await this.otpHandler.execute(
      new RequestOtpCommand(accountIndex, firstEntry.txnId)
    );

    if (!otpResponse || !otpResponse.txnId) {
      this.toastr.error('OTP request failed. Please try again.');
      return;
    }

    this.TxnId.set(otpResponse.txnId);
    this.Step.set(2);               
    this.toastr.success('OTP sent successfully');

  } catch (error) {
    console.error("Error in RequestOtp:", error);
    this.toastr.error('Something went wrong. Please try again.');
  } finally {
    this.Loading.set(false); 
  }
}

  async VerifyOtp(): Promise<void> {
  if (this.loginForm.get('otp')?.invalid) {
    this.toastr.error('Enter a valid 6-digit OTP');
    return;
  }

  this.Loading.set(true);

  try {
    const otp = this.loginForm.get('otp')!.value;
    const res = await this.verifyHandler.execute(
      new VerifyOtpCommand(this.TxnId(), otp, this.loginForm.get('mobile')!.value)
    );

    if (res && res.accounts) {
      this.AbhaAccounts.set(res.accounts);
      this.NoAbhaFound.set(false);  
      this.Step.set(3);             
      this.toastr.success('OTP verified successfully');
     
      await this.SearchPatientInDanphe(this.loginForm.get('mobile')!.value);
    }
  } catch (err) {
    console.error('OTP verification failed:', err);
    this.toastr.error('OTP verification failed. Please try again.');
  } finally {
    this.Loading.set(false); 
  }
}


async SearchPatientInDanphe(mobile: string): Promise<void> {
  this.Loading.set(true);

  try {
    const patientResults = await this.searchPatientHandler.execute(
      new SearchPatientQuery(mobile)
    );
    console.log('Patient results from Danphe:', patientResults);

    if (!patientResults || patientResults.length === 0) {
      this.toastr.info("No matching patient found in Danphe");
      this.Patients.set([]);
      return;
    }

    const flatPatients = patientResults.flat();
    this.Patients.set(flatPatients);
    console.log('Patients signal:', this.Patients());
    this.toastr.success("Patients loaded successfully.");
  } catch (error) {
    console.error("Error searching patient in Danphe:", error);
    this.toastr.error("Failed to search patient in Danphe");
    this.Patients.set([]);
  } finally {
    this.Loading.set(false);
  }
}

}
