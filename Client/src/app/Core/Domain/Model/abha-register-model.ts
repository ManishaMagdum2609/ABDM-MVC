export interface RegisterOtpResponse{
txnId: string;
message: string;
}
export interface VerifyRegistrationOtpResponse {  
  AbhaNumber: string;       
  AbhaAddress: string;       
  Name: string;              
  mobile?: string;          
  message?: string;      
}
