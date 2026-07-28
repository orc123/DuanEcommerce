import { OAuthService } from 'angular-oauth2-oidc';
import { Component, inject } from '@angular/core';
import { AuthService } from '@abp/ng.core';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';
import { BlockUIModule } from 'primeng/blockui';

@Component({
  selector: 'app-product',
  imports: [PanelModule, TableModule, PaginatorModule, BlockUIModule],
  template: `
    <p-panel header="Danh sách sản phẩm">
      <div class="grid">
        <div class="col-4">4</div>
        <div class="col-8">8</div>
      </div>
      <p-table #pnl [value]="items">
        <ng-template pTemplate="header">
          <tr>
            <th></th>
            <th></th>
            <th></th>
            <th></th>
          </tr>
        </ng-template>
        <ng-template pTemplate="body"> </ng-template>
      </p-table>
      <p-paginator
        [rows]="10"
        [totalRecords]="120"
        [rowsPerPageOptions]="[10, 29, 40]"
      ></p-paginator>
      <p-block-ui [blocked]="blockedPanel" [target]="pnl"></p-block-ui>
    </p-panel>
  `,
})
export class Product {
  oAuthService = inject(OAuthService);
  authService = inject(AuthService);

  blockedPanel: boolean = false;
  items = [];

  get HasLoggedIn(): boolean {
    return this.oAuthService.hasValidAccessToken();
  }

  login() {
    this.authService.navigateToLogin();
  }
}
