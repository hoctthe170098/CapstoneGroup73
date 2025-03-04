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
  