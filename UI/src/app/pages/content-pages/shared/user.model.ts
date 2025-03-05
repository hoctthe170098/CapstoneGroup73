export interface ForgotPassword {
    email: string;
  }
  
  
  export interface LoginRequest {
    username: string;
    password: string;
  }
  
  
  export interface LoginResponse {
    code: number;
    data: string;  // Token trả về từ API
    message: string;
  }
 // Interface gửi lên API
export interface ChangePasswordRequest {
  token: string;
  oldPassword: string;
  newPassword: string;
}

// Interface dùng trong form Angular (gồm confirmPassword)
export interface ChangePasswordForm {
  oldPassword: string;
  newPassword: string;
  confirmPassword: string;
}
