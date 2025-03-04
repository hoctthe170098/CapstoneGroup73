import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { UserService } from '../shared/user.service';
@Component({
  selector: 'app-forgot-password-page',
  templateUrl: './forgot-password-page.component.html',
  styleUrls: ['./forgot-password-page.component.scss']
})
export class ForgotPasswordPageComponent {
  @ViewChild('f') forgotPasswordForm!: NgForm;
  emailInvalid: boolean = false;
  message: string = '';

  constructor(private authService: UserService) {}

  validateEmail(email: string): boolean {
    const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return emailPattern.test(email);
  }

  onSubmit(event: Event) {
    event.preventDefault();
    const emailValue = this.forgotPasswordForm.value.email;
    
    if (!emailValue || !this.validateEmail(emailValue)) {
      this.emailInvalid = true;
      return;
    }
    
    this.emailInvalid = false;
    this.authService.forgotPassword({ email: emailValue }).subscribe({
      next: () => {
        this.message = 'Vui lòng kiểm tra email để đặt lại mật khẩu.';
        this.forgotPasswordForm.reset();
      },
      error: () => {
        this.message = 'Có lỗi xảy ra, vui lòng thử lại!';
      }
    });
  }

  clearMessage() {
    this.message = '';
  }
}
