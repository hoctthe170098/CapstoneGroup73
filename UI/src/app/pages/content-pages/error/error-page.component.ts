import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../shared/user.service';
import { UserRole } from '../shared/user.model';
import { ro } from 'date-fns/locale';

@Component({
  selector: 'app-error-page',
  templateUrl: './error-page.component.html',
  styleUrls: ['./error-page.component.scss']
})
export class ErrorPageComponent {
  constructor(private router: Router, private authService: UserService,) { }
  ReturnHome() {
    var roles = this.authService.getRoleNames();
    if (roles == null) this.router.navigate(['/pages/login'])
    var role = roles[0];
    if (role == UserRole.Administrator)
      this.router.navigate(['/coso']);
    else if (role == UserRole.CampusManager) {
      this.router.navigate(['/lophoc']);
    } else if (role == UserRole.LearningManager) {
      this.router.navigate(['/chuongtrinh']);
    } else if (role == UserRole.Teacher) {
      this.router.navigate(['/lopdangday']);
    } else if (role == UserRole.Student) {
      this.router.navigate(['/lopdanghoc']);
    } else this.router.navigate(['/pages/error'])
  }
}