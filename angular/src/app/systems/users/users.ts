import { UserDto, UsersService } from '@/app/proxy/users';
import { NotificationService } from '@/app/shared/services/notification.service';
import { PagedResultDto } from '@abp/ng.core';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { ConfirmationService } from 'primeng/api';
import { Subject, takeUntil } from 'rxjs';
import { PaginatorModule } from 'primeng/paginator';
import { BlockUIModule } from 'primeng/blockui';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { Dialog } from 'primeng/dialog';
import { TableModule } from 'primeng/table';
import { PanelModule } from 'primeng/panel';
import { ButtonModule } from 'primeng/button';
import { FormsModule } from '@angular/forms';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import { MessageModule } from 'primeng/message';
import { BadgeModule } from 'primeng/badge';
import { DatePipe, DecimalPipe } from '@angular/common';
import { InputTextModule } from 'primeng/inputtext';
import { UserDetail } from './user-detail';

@Component({
  selector: 'app-user',
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
              <th>Tài khoản</th>
              <th>Email</th>
              <th>Số ĐT</th>
              <th>Ngày tham gia</th>
              <th style="width: 150px">Trạng thái</th>
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
              <td>{{ row.userName }}</td>
              <td>{{ row.email }}</td>
              <td>{{ row.phoneNumber }}</td>
              <td>{{ row!.creationTime | date: 'dd-MM-yyyy hh:mm' }}</td>
              <td style="width: 150px">
                @if (row.isActive === true) {
                  <p-badge value="Kích hoạt" severity="success"></p-badge>
                } @else {
                  <p-badge value="Khoá" severity="danger"></p-badge>
                }
              </td>
              <td>
                <!-- <button
          pButton
          pRipple
          type="button"
          icon="pi pi-key"
          pTooltip="Đặt mật khẩu"
          tooltipPosition="top"
          class="p-button-rounded p-button-text"
          (click)="setPassword(row.id)"
        ></button>
        
        <button
          pButton
          pRipple
          type="button"
          icon="pi pi-users"
          pTooltip="Gán vai trò"
          tooltipPosition="top"
          (click)="assignRole(row.id)"
          class="p-button-rounded p-button-text"
        ></button> -->
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
        <app-user-detail [userId]="userId" (saveChange)="saveData()"></app-user-detail>
      </p-dialog>
    }
  `,
  imports: [
    PanelModule,
    TableModule,
    PaginatorModule,
    BlockUIModule,
    ButtonModule,
    FormsModule,
    ProgressSpinnerModule,
    DynamicDialogModule,
    Dialog,
    MessageModule,
    BadgeModule,
    DecimalPipe,
    DatePipe,
    InputTextModule,
    UserDetail,
  ],
})
export class Users implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();

  usersService = inject(UsersService);
  notificationService = inject(NotificationService);
  confirmationService = inject(ConfirmationService);

  blockedPanel: boolean = false;
  items: UserDto[] = [];

  public skipCount: number = 0;
  public maxResultCount: number = 10;
  public totalCount: number | undefined = 0;
  public selectedItems: UserDto[] = [];
  public userId: string = '';

  // Filters

  keyword: string = '';

  visibleForm = false;
  visibleFormPermission = false;
  headerTitle = '';

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.toggleBlockUI(true);
    this.usersService
      .getListWithFilter({
        keyword: this.keyword,
        maxResultCount: this.maxResultCount,
        skipCount: this.skipCount,
      })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: PagedResultDto<UserDto>) => {
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
    this.headerTitle = 'Thêm mới Người dùng';
    this.visibleForm = true;
    this.userId = '';
  }

  showEditModal() {
    if (this.selectedItems.length == 0 || this.selectedItems.length > 1) {
      this.notificationService.showError('Bạn phải chọn 1 bản ghi');
      return;
    }
    this.headerTitle = 'Chỉnh sửa Người dùng';
    this.visibleForm = true;
    this.userId = this.selectedItems[0].id!;
  }

  saveData() {
    this.visibleForm = false;
    this.loadData();
    this.selectedItems = [];
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
    this.usersService
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
