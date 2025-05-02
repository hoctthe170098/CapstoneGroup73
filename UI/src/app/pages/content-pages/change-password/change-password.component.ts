import { Component,ChangeDetectorRef } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { UserService } from '../shared/user.service';
import { ToastrService } from 'ngx-toastr';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router } from '@angular/router';
import { ChangePasswordRequest, UserRole } from '../shared/user.model';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent {
  changePasswordFormSubmitted = false;
  showPassword = {
    currentPassword: false,
    newPassword: false,
    confirmPassword: false
  };

  changePasswordForm = new FormGroup({
    oldPassword: new FormControl('', [Validators.required]),
    newPassword: new FormControl('', [Validators.required, Validators.minLength(6)]),
    confirmPassword: new FormControl('', [Validators.required])
  });

  constructor(
    private userService: UserService,
    private toastr: ToastrService,
    private spinner: NgxSpinnerService,
    private router: Router,
    private authService: UserService,
    private cdr: ChangeDetectorRef
  ) {}
  ngOnInit(): void {
    
  }
  get cpf() {
    return this.changePasswordForm.controls;
  }
  
  togglePassword(field: string) {
    this.showPassword[field] = !this.showPassword[field];
  }

  onSubmit() {
    this.changePasswordFormSubmitted = true;
  
    if (this.changePasswordForm.invalid) {
      this.toastr.error('Vui lòng nhập đầy đủ thông tin!', 'Lỗi');
      return;
    }
  
    if (this.changePasswordForm.value.newPassword !== this.changePasswordForm.value.confirmPassword) {
      this.toastr.error('Mật khẩu xác nhận không khớp!', 'Lỗi');
      return;
    }
  
    this.spinner.show();
  
    const token = localStorage.getItem('token') || ''; 
  
    const request: ChangePasswordRequest = {
      token: token, 
      oldPassword: this.changePasswordForm.value.oldPassword!,
      newPassword: this.changePasswordForm.value.newPassword!
    };
  
    this.userService.changePassword(request).subscribe(
      (res: any) => {    
        if (res.code === 404) {
          this.router.navigate(['/pages/error'])
          return;
        }
        if (res && !res.isError) {
          this.toastr.success('Đổi mật khẩu thành công!', 'Thành công');
          setTimeout(() => {
            this.router.navigate(['/pages/change-password']).then(() => {
              window.location.reload(); // Refresh sau khi hiển thị toast
            });
          }, 1500);
        } else if (res.isError && res.message.includes('Incorrect password')) {  
          this.toastr.error('Mật khẩu cũ không đúng!', 'Lỗi');
        } else {
          this.toastr.error(res?.message || 'Lỗi không xác định!', 'Lỗi');
        }
        this.spinner.hide();
      },
      (error) => {
        console.error('API Error:', error);
        this.toastr.error('Đổi mật khẩu thất bại. Vui lòng thử lại!', 'Lỗi');
        this.spinner.hide();
      }
    );    
  }
  onCancel() {
    var role = this.authService.getRoleNames()[0];
          if(role==UserRole.Administrator)
          this.router.navigate(['/coso']);
        else if(role==UserRole.CampusManager){
          this.router.navigate(['/lophoc']);
        }else if(role==UserRole.LearningManager){
          this.router.navigate(['/chuongtrinh']);
        }else if(role==UserRole.Teacher){
          this.router.navigate(['/lopdangday']);
        }else if(role==UserRole.Student){
          this.router.navigate(['/lopdanghoc']);
        }else this.router.navigate(['/pages/error'])
  }
}
