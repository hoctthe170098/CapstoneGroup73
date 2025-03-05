import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { UserService } from '../shared/user.service';
import { ToastrService } from 'ngx-toastr';
@Component({
  selector: 'app-forgot-password-page',
  templateUrl: './forgot-password-page.component.html',
  styleUrls: ['./forgot-password-page.component.scss']
})
export class ForgotPasswordPageComponent {
  @ViewChild('f') forgotPasswordForm!: NgForm;
  emailInvalid: boolean = false;
  message: string = '';

  constructor(private authService: UserService,private toastr: ToastrService) {}

  validateEmail(email: string): boolean {
    const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return emailPattern.test(email);
  }

  onSubmit(event: Event) {
    event.preventDefault();
    const emailValue = this.forgotPasswordForm.value.email;
  
    if (!emailValue || !this.validateEmail(emailValue)) {
      this.emailInvalid = true;
      //this.toastr.error('Vui lòng nhập email hợp lệ!', 'Lỗi');
      return;
    }
  
    this.emailInvalid = false;
    this.authService.forgotPassword({ email: emailValue }).subscribe({
      next: (response) => {
        if (response.code === 0 && response.message.includes("Email không khớp")) {
          this.toastr.warning(response.message, 'Cảnh báo');
        } else {
          this.toastr.success('Vui lòng kiểm tra email để đặt lại mật khẩu.', 'Thành công');
          this.forgotPasswordForm.reset();
        }
      },
      error: () => {
        this.toastr.error('Có lỗi xảy ra, vui lòng thử lại!', 'Lỗi');
      }
    });
  }
  

  clearMessage() {
    this.message = '';
  }
}
