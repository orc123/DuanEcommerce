import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { LOGIN_URL } from '../constants/urls.const';
export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  // 1. Kiểm tra xem người dùng đã đăng nhập chưa
  if (authService.isAuthenticated()) {
    return true; // Cho phép truy cập vào Route
  }
  // 2. Nếu chưa đăng nhập: Lưu lại URL người dùng muốn vào để sau khi login xong có thể redirect lại
  return router.createUrlTree([LOGIN_URL], {
    queryParams: { returnUrl: state.url },
  });
};
