import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { UserService } from 'app/pages/content-pages/shared/user.service';

@Injectable()
export class AuthGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router,private userService : UserService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot):boolean {
    const token = localStorage.getItem('token');
    if (!token) {
      // Chuyển hướng về trang login nếu không có token
      this.router.navigate(['/pages/login']);
      return false;
    }
    if(this.userService.isTokenExpired()){
      // Xóa token khỏi localStorage và chuyển hướng về trang login nếu token hết hạn
      localStorage.removeItem('token');
      this.router.navigate(['/pages/login']);
      return false;
    }
    // Giả sử bạn có một hàm để giải mã token và lấy thông tin vai trò
    const userRole = this.userService.getRoleNames()[0];
    const requiredRole = route.data['role'] as string[] ;
    // Kiểm tra quyền hạn của người dùng
    if (requiredRole &&  !requiredRole.includes(userRole)) {
      // Chuyển hướng đến trang forbidden nếu vai trò không phù hợp
      this.router.navigate(['/pages/error']);
      return false;
    }
    return true;
  }
}
