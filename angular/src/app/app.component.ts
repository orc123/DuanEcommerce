import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LoaderBarComponent } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-root',
  template: `
    <abp-loader-bar />
    <router-outlet />
  `,
  imports: [LoaderBarComponent, RouterOutlet],
})
export class AppComponent {}

