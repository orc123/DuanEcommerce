import { Component, inject, OnInit } from '@angular/core';
import { LocalizationPipe } from '@abp/ng.core';
import { AuthService } from '../shared/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  imports: [LocalizationPipe],
})
export class HomeComponent implements OnInit {
  private authService = inject(AuthService);
  router = inject(Router);
  public isLoginIn = false;

  ngOnInit(): void {
    this.isLoginIn = this.authService.isAuthenticated();
    if (this.isLoginIn == false) {
      this.login();
    }
  }

  login() {
    this.router.navigate(['/auth/login']);
    //this.authService.navigateToLogin();
  }
}
