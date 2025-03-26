export interface ForgotPassword {
    email: string;
  }
  
  
  export interface LoginRequest {
    username: string;
    password: string;
  }
  
  
  export interface LoginResponse {
    isError: boolean;      // Thêm để check lỗi
    code: number;
    data: string;  
    message: string;
    errors?: string[];     // Có thể optional nếu đôi khi không có
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
export enum UserRole {
  Administrator = 'Administrator',
  CampusManager = 'CampusManager',
  LearningManager = 'LearningManager',
  Student = 'Student',
  Teacher= 'Teacher'
}
