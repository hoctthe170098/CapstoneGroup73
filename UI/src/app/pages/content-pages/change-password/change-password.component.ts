import { Component } from '@angular/core';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent {
  currentPassword: string = '';
  newPassword: string = '';
  confirmPassword: string = '';

  onChangePassword() {
    console.log("Password changed!");
  }

  togglePassword(fieldId: string) {
    const inputField = document.querySelector(`input[name="${fieldId}"]`) as HTMLInputElement;
    if (inputField) {
      inputField.type = inputField.type === 'password' ? 'text' : 'password';
    }
  }
}
