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

  constructor(private authService: UserService, private toastr: ToastrService) {}

  // Sử dụng regex yêu cầu email có phần mở rộng (TLD)
  validateEmail(email: string): boolean {
    const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return emailPattern.test(email);
  }

  onSubmit(event: Event) {
    event.preventDefault();
    // Đánh dấu tất cả các trường là touched để hiển thị thông báo lỗi nếu có
    this.forgotPasswordForm.control.markAllAsTouched();
    const emailValue = this.forgotPasswordForm.value.email;

    // Kiểm tra email theo custom validateEmail
    if (!emailValue) {
      this.emailInvalid = true;
      return;
    }

    this.emailInvalid = false;
    this.authService.forgotPassword({ email: emailValue }).subscribe({
      next: (response) => {
        if (response.isError) {
          this.toastr.warning(response.message, 'Cảnh báo');
        } else {
          this.toastr.success(response.message, 'Thành công');
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
