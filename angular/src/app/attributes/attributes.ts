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
import { NotificationService } from '../shared/services/notification.service';
import { BadgeModule } from 'primeng/badge';
import { ConfirmationService } from 'primeng/api';
import { ProductAttributeDto, ProductAttributesService } from '../proxy/product-attributes';
import { AttributeType } from '../proxy/duan-ecommerce/product-attributes';
import { AttributeDetail } from './attribute-detail';

@Component({
  selector: 'app-attributes',
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
    AttributeDetail,
  ],
  template: `
    <p-panel header="Danh sách thuộc tính sản phẩm">
      <div class="grid">
        <div class="col-4">
          <button pButton icon="fa fa-plus" label="Thêm mới" (click)="showAddModal()"></button>
          @if (selectedItems.length === 1) {
            <button
              pButton
              icon="fa fa-minus"
              class="ml-1 p-button-help"
              label="Sửa"
              (click)="showEditModal()"
            ></button>
          }

          @if (selectedItems.length > 0) {
            <button
              pButton
              icon="fa fa-minus"
              class="ml-1 p-button-danger"
              label="Xóa"
              (click)="deleteItems()"
            ></button>
          }
        </div>
        <div class="col-8">
          <!-- <div class="formgroup-inline">
            <div class="field">
              <label for="txt-keyword" class="p-sr-only">Từ khóa</label>
              <input id="txt-keyword" pInputText type="text" placeholder="Gõ từ khóa" />
            </div>
            <button
              type="button"
              pButton
              (click)="loadData()"
              icon="fa fa-search"
              label="Tìm"
            ></button>
          </div> -->
        </div>
      </div>
      <p-table #pnl [value]="items" [(selection)]="selectedItems" selectionMode="multiple">
        <ng-template pTemplate="header">
          <tr>
            <th style="width: 10px">
              <p-tableHeaderCheckbox></p-tableHeaderCheckbox>
            </th>
            <th>Mã</th>
            <th>Kiểu dữ liệu</th>
            <th>Nhãn</th>
            <th>Thứ tự</th>
            <th>Hiển thị</th>
            <th>Bắt buộc nhập</th>
            <th>Duy nhất</th>
            <th>Kích hoạt</th>
            <th></th>
          </tr>
        </ng-template>
        <ng-template #body let-row>
          <tr [pSelectableRow]="row">
            <td style="width: 10px">
              <span class="ui-column-title"></span>
              <p-tableCheckbox [value]="row"></p-tableCheckbox>
            </td>
            <td>{{ row.code }}</td>
            <td>{{ getAttributeTypeName(row.dataType) }}</td>
            <td>{{ row.label }}</td>
            <td>{{ row.sortOrder }}</td>
            <td>
              @if (row.visibility === true) {
                <p-badge value="Kích hoạt" severity="success"></p-badge>
              } @else {
                <p-badge value="Khoá" severity="danger"></p-badge>
              }
            </td>
            <td>
              @if (row.isRequired === true) {
                <p-badge value="Có" severity="success"></p-badge>
              } @else {
                <p-badge value="Không" severity="danger"></p-badge>
              }
            </td>
            <td>
              @if (row.isUnique === true) {
                <p-badge value="Có" severity="success"></p-badge>
              } @else {
                <p-badge value="Không" severity="danger"></p-badge>
              }
            </td>
            <td>
              @if (row.isActive === true) {
                <p-badge value="Kích hoạt" severity="success"></p-badge>
              } @else {
                <p-badge value="Khoá" severity="danger"></p-badge>
              }
            </td>
          </tr>
        </ng-template>
        <ng-template pTemplate="summary">
          <div style="text-align: left">Tổng số bản ghi: {{ totalCount | number }}</div>
        </ng-template>
      </p-table>
      <p-paginator
        [rows]="maxResultCount"
        [totalRecords]="totalCount"
        [rowsPerPageOptions]="[10, 20, 30, 50, 100]"
        (onPageChange)="pageChanged($event)"
      ></p-paginator>
      <p-block-ui [blocked]="blockedPanel" [target]="pnl">
        <p-progressSpinner></p-progressSpinner>
      </p-block-ui>
    </p-panel>

    @if (visibleForm) {
      <p-dialog
        [header]="headerTitle"
        [modal]="true"
        [(visible)]="visibleForm"
        [style]="{ width: '70%' }"
      >
        <app-attribute-detail
          [attributeId]="attributeId"
          (saveChange)="saveData()"
        ></app-attribute-detail>
      </p-dialog>
    }
  `,
  providers: [],
})
export class Attributes implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();

  productAttributesService = inject(ProductAttributesService);
  notificationService = inject(NotificationService);
  confirmationService = inject(ConfirmationService);

  blockedPanel: boolean = false;
  items: ProductAttributeDto[] = [];

  public skipCount: number = 0;
  public maxResultCount: number = 10;
  public totalCount: number | undefined = 0;
  public selectedItems: ProductAttributeDto[] = [];

  // Filters

  keyword: string = '';
  categoryId: string = '';

  visibleForm = false;
  attributeId: string = '';
  headerTitle = '';

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.toggleBlockUI(true);
    this.productAttributesService
      .getListFilter({
        keyword: this.keyword,
        maxResultCount: this.maxResultCount,
        skipCount: this.skipCount,
      })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: PagedResultDto<ProductAttributeDto>) => {
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
    this.headerTitle = 'Thêm mới thuộc tính sản phẩm';
    this.visibleForm = true;
    this.attributeId = '';
  }

  showEditModal() {
    if (this.selectedItems.length == 0 || this.selectedItems.length > 1) {
      this.notificationService.showError('Bạn phải chọn 1 bản ghi');
      return;
    }
    this.headerTitle = 'Chỉnh sửa thuộc tính sản phẩm';
    this.visibleForm = true;
    this.attributeId = this.selectedItems[0].id!;
  }

  saveData() {
    this.visibleForm = false;
    this.loadData();
    this.selectedItems = [];
  }

  getAttributeType(value: number) {
    return AttributeType[value];
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
    this.productAttributesService
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

  getAttributeTypeName(value: number) {
    return AttributeType[value];
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
