import { Component } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../shared/user.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { LoginRequest} from '../shared/user.model';

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss']
})
export class LoginPageComponent {
  loginFormSubmitted = false;
  isLoginFailed = false;
  errorMessage = '';

  loginForm = new FormGroup({
    username: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required]),
    rememberMe: new FormControl(true)
  });

  constructor(private router: Router, private authService: UserService, private spinner: NgxSpinnerService,private toastr: ToastrService) {}

  get lf() {
    return this.loginForm.controls;
  }

  onSubmit() {
    this.loginFormSubmitted = true;
    this.isLoginFailed = false; // Reset lỗi trước khi gửi yêu cầu

    if (this.loginForm.invalid) {
      this.toastr.error('Vui lòng nhập đầy đủ thông tin', 'Lỗi');
      return;
    }

    this.spinner.show();

    const request: LoginRequest = {
      username: this.loginForm.value.username!,
      password: this.loginForm.value.password!
    };

    this.authService.login(request).subscribe(
      (res) => {
        console.log('Login response:', res);

        if (res.code === 200) {
          localStorage.setItem('token', res.data);
          this.toastr.success('Đăng nhập thành công!', 'Thành công');
          this.router.navigate(['/dashboard/dashboard1']);
        } else {
          this.isLoginFailed = true; 
          
          this.toastr.error(res.message, 'Lỗi');
        }
        this.spinner.hide();
      },
      (error) => {
        this.isLoginFailed = true; 
        
        this.toastr.error('Đăng nhập thất bại. Vui lòng thử lại!', 'Lỗi');
        this.spinner.hide();
        console.error('Login failed:', error);
      }
    );
  }
}
