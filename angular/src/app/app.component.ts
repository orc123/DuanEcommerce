import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { LoaderBarComponent } from '@abp/ng.theme.shared';
import { AuthService } from './shared/services/auth.service';
import { LOGIN_URL } from './shared/constants/urls.const';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-root',
  template: `
    <abp-loader-bar />
    <router-outlet />
    <p-confirmDialog />
    <p-toast />
  `,
  imports: [LoaderBarComponent, RouterOutlet, ConfirmDialogModule, ToastModule],
  providers: [ConfirmationService],
})
export class AppComponent implements OnInit {
  authService = inject(AuthService);
  router = inject(Router);

  ngOnInit(): void {
    if (this.authService.isAuthenticated() == false) {
      this.router.navigate([LOGIN_URL]);
    }
  }
}
