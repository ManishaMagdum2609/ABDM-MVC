import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { AbhaRegistrationApiService } from '../../../Infrastructure/Services/abhaRegistration-api.service';
import {  RequestRegistrationOtpHandler } from '../../../Application/Abha/Commands/Handler/request-registration-otp.handler';
import { VerifyRegistrationOtpHandler } from '../../../Application/Abha/Commands/Handler/verify-registration-otp.handler';
import { ToastrService } from 'ngx-toastr';
import { RequestRegistationOtpCommand } from '../../../Application/Abha/Commands/request-registration-otp.command';
import { VerifyRegistrationOtpCommand } from '../../../Application/Abha/Commands/verify-registration-otp.command';
import { VerifyRegistrationOtpResponse } from '../../../Domain/Model/abha-register-model';

@Component({
  selector: 'app-abha-registration',
  imports: [CommonModule,ReactiveFormsModule ,FormsModule ],
  templateUrl: './abha-registration.html',
  styleUrl: './abha-registration.css'
})
export class AbhaRegistration {
  abhaForm!: FormGroup;
  ShowCreateAbha = signal(false);
  Aadhar = signal('');
  AadharOtp = signal('');
  NewMobile = signal('');
  CreateStep = signal(1); 
  AbhaSearched = signal(false);    
  NoAbhaFound = signal(false); 
  ShowCreate :boolean=false;
  Loading = signal(false);
  TxnId = signal('');
  Aadhaar = signal('');
  VerifiedAbha: VerifyRegistrationOtpResponse | null = null;
  private requestOtpHandler: RequestRegistrationOtpHandler;
  private verifyOtpHandler: VerifyRegistrationOtpHandler;
  constructor(private fb: FormBuilder, 
  private abhaService: AbhaRegistrationApiService,
  private toastr: ToastrService, ){
    this.requestOtpHandler = new RequestRegistrationOtpHandler(abhaService);
    this.verifyOtpHandler = new VerifyRegistrationOtpHandler(abhaService);
  }
ngOnInit(): void {
    this.abhaForm = this.fb.group({
      aadhar: ['', [Validators.required, Validators.pattern(/^\d{12}$/)]],
      otp: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]],
      mobile: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
    });
  }
toggleCreateAbhaBox() {
    this.ShowCreateAbha.set(!this.ShowCreateAbha());
  }

 async RequestOtp(): Promise<void> {
  const aadhaar = this.abhaForm.get('aadhar')?.value; 

  if (!aadhaar || aadhaar.length !== 12) {
    this.toastr.error('Enter a valid 12-digit Aadhaar number');
    return;
  }

  this.Loading.set(true);
  try {
    const command = new RequestRegistationOtpCommand(aadhaar);
    const res = await this.requestOtpHandler.execute(command);

    if (!res || !res.txnId) {
      this.toastr.error('OTP request failed. Please try again.');
      return;
    }
    this.TxnId.set(res.txnId);
    this.CreateStep.set(2);
    this.toastr.success('OTP sent successfully');
  } catch (err) {
    console.error('Error in RequestOtp:', err);
    this.toastr.error('Something went wrong. Please try again.');
  } finally {
    this.Loading.set(false);
  }
 }

 async VerifyAadharOtp(): Promise<void> {
   const aadhaar = this.abhaForm.get('aadhar')?.value;
   const otp = this.abhaForm.get('otp')?.value;
   const mobile = this.abhaForm.get('mobile')?.value;

   if (!aadhaar || aadhaar.length !== 12) {
    this.toastr.error('Enter a valid 12-digit Aadhaar number');
    return;
   }
   if (!otp || otp.length !== 6) {
    this.toastr.error('Enter a valid 6-digit OTP');
    return;
   }
   if (!mobile || mobile.length !== 10) {
    this.toastr.error('Enter a valid 10-digit mobile number');
    return;
   }
   this.Loading.set(true);
   try {
    const command = new VerifyRegistrationOtpCommand(
      this.TxnId(), 
      otp,
      mobile
    );
   const res = await this.verifyOtpHandler.execute(command);
   if (!res) {
   this.toastr.error('OTP verification failed. Please try again.');
   return;
    }
    this.VerifiedAbha = {
    AbhaNumber: res.abhaProfile?.abhaNumber || res.abhaProfile?.preferredAddress?.split('@')[0] || '',
    AbhaAddress: res.abhaProfile?.preferredAddress || '',
    Name: `${res.abhaProfile?.firstName || ''} ${res.abhaProfile?.middleName || ''} ${res.abhaProfile?.lastName || ''}`.trim(),
    mobile:res.abhaProfile?.mobile,
    message: res.message
    };
    if (res.message) {
    this.toastr.info(res.message);
    }
    this.CreateStep.set(3);
   } catch (err) {
    console.error('Error verifying registration OTP:', err);
    this.toastr.error('Something went wrong. Please try again.');
   } finally {
    this.Loading.set(false);
   }
 }


}
