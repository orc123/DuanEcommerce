import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LoaderBarComponent } from '@abp/ng.theme.shared';
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
export class AppComponent {}
