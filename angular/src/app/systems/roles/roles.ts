import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';
import { BlockUIModule } from 'primeng/blockui';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { Subject, takeUntil } from 'rxjs';
import { PagedResultDto } from '@abp/ng.core';
import { DecimalPipe } from '@angular/common';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import { Dialog } from 'primeng/dialog';
import { MessageModule } from 'primeng/message';
import { NotificationService } from '../../shared/services/notification.service';
import { BadgeModule } from 'primeng/badge';
import { ConfirmationService } from 'primeng/api';
import { RoleDto, RolesService } from '../../proxy/roles';
import { RoleDetail } from './role-detail';
import { PermissionGrant } from './permission-grant';

@Component({
  selector: 'app-roles',
  imports: [
    PanelModule,
    TableModule,
    PaginatorModule,
    BlockUIModule,
    DecimalPipe,
    ButtonModule,
    DropdownModule,
    FormsModule,
    InputTextModule,
    ProgressSpinnerModule,
    DynamicDialogModule,
    Dialog,
    MessageModule,
    BadgeModule,
    RoleDetail,
    PermissionGrant,
  ],
  template: `
    <div class="animated fadeIn">
      <p-panel #pnl [style]="{ 'margin-bottom': '10px' }">
        <p-header> Quản lý quyền </p-header>
        <div class="grid">
          <div class="col-6">
            <button
              pButton
              type="button"
              label="Thêm"
              icon="fa fa-plus"
              (click)="showAddModal()"
              class="ml-1 p-button-info"
            ></button>
            @if (selectedItems.length > 0) {
              <button
                pButton
                type="button"
                label="Xóa"
                icon="fa fa-trash"
                class="ml-1 p-button-danger"
                (click)="deleteItems()"
              ></button>
            }

            @if (selectedItems.length === 1) {
              <button
                pButton
                type="button"
                label="Sửa"
                icon="fa fa-edit"
                class="ml-1 p-button-help"
                (click)="showEditModal()"
              ></button>
            }
          </div>
          <div class="col-6">
            <div class="formgroup-inline">
              <div class="field">
                <input
                  id="txtKeyword"
                  pInputText
                  (keyup.enter)="loadData()"
                  [(ngModel)]="keyword"
                  placeholder="Nhập tên nhóm..."
                  type="text"
                />
              </div>
              <button type="button" pButton (click)="loadData()">Tìm</button>
            </div>
          </div>
        </div>
        <p-table
          #dt
          [value]="items"
          selectionMode="multiple"
          dataKey="id"
          [(selection)]="selectedItems"
          [metaKeySelection]="true"
          [responsive]="true"
        >
          <ng-template pTemplate="header">
            <tr>
              <th style="width: 10px">
                <p-tableHeaderCheckbox></p-tableHeaderCheckbox>
              </th>
              <th>Tên</th>
              <th>Mô tả</th>
              <th></th>
            </tr>
          </ng-template>
          <ng-template pTemplate="body" let-row>
            <tr [pSelectableRow]="row">
              <td style="width: 10px">
                <span class="ui-column-title"></span>
                <p-tableCheckbox [value]="row"></p-tableCheckbox>
              </td>
              <td>{{ row.name }}</td>
              <td>{{ row.description }}</td>
              <td>
                <button pButton (click)="showPermissionModal(row.id, row.name)">Phân quyền</button>
              </td>
            </tr>
          </ng-template>
          <ng-template pTemplate="summary">
            <div style="text-align: left">Tổng số: {{ totalCount | number }}</div>
          </ng-template>
        </p-table>

        <p-paginator
          [rows]="maxResultCount"
          [totalRecords]="totalCount"
          (onPageChange)="pageChanged($event)"
          [rowsPerPageOptions]="[10, 20, 50, 100]"
        ></p-paginator>

        <p-blockUI [target]="pnl" [blocked]="blockedPanel">
          <p-progressSpinner
            [style]="{
              width: '100px',
              height: '100px',
              position: 'absolute',
              top: '25%',
              left: '50%',
            }"
            strokeWidth="2"
            animationDuration=".5s"
          ></p-progressSpinner>
        </p-blockUI>
      </p-panel>
    </div>

    @if (visibleForm) {
      <p-dialog
        [header]="headerTitle"
        [modal]="true"
        [(visible)]="visibleForm"
        [style]="{ width: '70%' }"
      >
        <app-role-detail [roleId]="roleId" (saveChange)="saveData()"></app-role-detail>
      </p-dialog>
    }

    @if (visibleFormPermission) {
      <p-dialog
        header="Phân quyền"
        [modal]="true"
        [(visible)]="visibleFormPermission"
        [style]="{ width: '70%' }"
      >
        <app-permission-grant
          [providerKey]="roleName"
          (saveChange)="saveDataPermission()"
        ></app-permission-grant>
      </p-dialog>
    }
  `,
  providers: [],
})
export class Roles implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();

  roleService = inject(RolesService);
  notificationService = inject(NotificationService);
  confirmationService = inject(ConfirmationService);

  blockedPanel: boolean = false;
  items: RoleDto[] = [];

  public skipCount: number = 0;
  public maxResultCount: number = 10;
  public totalCount: number | undefined = 0;
  public selectedItems: RoleDto[] = [];

  // Filters

  keyword: string = '';
  categoryId: string = '';

  visibleForm = false;
  visibleFormPermission = false;
  roleId: string = '';
  roleName = '';
  headerTitle = '';

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.toggleBlockUI(true);
    this.roleService
      .getListFilter({
        keyword: this.keyword,
        maxResultCount: this.maxResultCount,
        skipCount: this.skipCount,
      })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: PagedResultDto<RoleDto>) => {
          this.items = response.items ?? [];
          this.totalCount = response.totalCount;
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  pageChanged(event: any): void {
    this.skipCount = (event.page - 1) * this.maxResultCount;
    this.maxResultCount = event.rows;
    this.loadData();
  }

  showAddModal() {
    this.headerTitle = 'Thêm mới Quyền';
    this.visibleForm = true;
    this.roleId = '';
  }

  showEditModal() {
    if (this.selectedItems.length == 0 || this.selectedItems.length > 1) {
      this.notificationService.showError('Bạn phải chọn 1 bản ghi');
      return;
    }
    this.headerTitle = 'Chỉnh sửa Quyền';
    this.visibleForm = true;
    this.roleId = this.selectedItems[0].id!;
  }

  saveData() {
    this.visibleForm = false;
    this.loadData();
    this.selectedItems = [];
  }

  showPermissionModal(id: string, name: string) {
    this.visibleFormPermission = true;
    this.roleName = name;
  }

  saveDataPermission() {
    this.visibleFormPermission = false;
    this.loadData();
    this.selectedItems = [];
    this.roleName = '';
  }

  deleteItems() {
    if (this.selectedItems.length == 0) {
      this.notificationService.showError('Bạn phải chọn ít nhất 1 bản ghi');
      return;
    }

    var ids = this.selectedItems.map(x => x.id!);

    this.confirmationService.confirm({
      header: 'Xóa các bản ghi đã chọn?',
      message: 'Bạn có muốn xoá các bản ghi đã chọn?',
      rejectButtonProps: {
        label: 'Hủy',
        severity: 'secondary',
        outlined: true,
      },
      acceptButtonProps: {
        label: 'Xóa',
      },
      accept: () => {
        this.deleteItemsConfirmed(ids);
      },
    });
  }

  deleteItemsConfirmed(ids: string[]) {
    this.toggleBlockUI(true);
    this.roleService
      .deleteMultiple(ids)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: () => {
          this.notificationService.showSuccess('Xóa các bản ghi đã chọn thành công');
          this.loadData();
          this.selectedItems = [];
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.blockedPanel = true;
    } else {
      setTimeout(() => {
        this.blockedPanel = false;
      }, 1000);
    }
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
}
