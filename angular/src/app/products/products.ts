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
import { ProductDto, ProductsService } from '../proxy/products';
import { PagedResultDto } from '@abp/ng.core';
import { DecimalPipe } from '@angular/common';
import { ProductCategoriesService, ProductCategoryDto } from '../proxy/product-categories';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import { NotificationService } from '../shared/services/notification.service';
import { ProductDetail } from './product-detail';
import { Dialog } from 'primeng/dialog';
import { MessageModule } from 'primeng/message';

@Component({
  selector: 'app-products',
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
    ProductDetail,
    MessageModule,
  ],
  template: `
    <p-panel header="Danh sách sản phẩm">
      <div class="grid">
        <div class="col-4">
          <button pButton icon="fa fa-plus" label="Thêm mới" (click)="showAddModal()"></button>
        </div>
        <div class="col-8">
          <div class="formgroup-inline">
            <div class="field">
              <label for="txt-keyword" class="p-sr-only">Từ khóa</label>
              <input id="txt-keyword" pInputText type="text" placeholder="Gõ từ khóa" />
            </div>
            <div class="field">
              <p-dropdown
                [options]="productCategories"
                [(ngModel)]="categoryId"
                optionValue="value"
                optionLabel="label"
                placeholder="Chọn danh mục"
              ></p-dropdown>
            </div>
            <button
              type="button"
              pButton
              (click)="loadData()"
              icon="fa fa-search"
              label="Tìm"
            ></button>
          </div>
        </div>
      </div>
      <p-table #pnl [value]="items">
        <ng-template pTemplate="header">
          <tr>
            <th>Mã</th>
            <th>SKU</th>
            <th>Tên</th>
            <th>Loại</th>
            <th>Tên danh mục</th>
            <th>Thứ tự</th>
            <th>Hiển thị</th>
            <th>Kích hoạt</th>
          </tr>
        </ng-template>
        <ng-template #body let-product>
          <tr>
            <td>{{ product.code }}</td>
            <td>{{ product.sku }}</td>
            <td>{{ product.name }}</td>
            <td>{{ product.productType }}</td>
            <td>{{ product.categoryId }}</td>
            <td>{{ product.sortOrder }}</td>
            <td>{{ product.visibility }}</td>
            <td>{{ product.isActive }}</td>
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

    <p-dialog
      [header]="headerTitle"
      [modal]="true"
      [(visible)]="visibleForm"
      [style]="{ width: '70%' }"
    >
      <app-product-detail [id]="productId" (saveChange)="saveData()"> </app-product-detail>
    </p-dialog>
  `,
})
export class Products implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();

  productService = inject(ProductsService);
  productCategoryService = inject(ProductCategoriesService);
  notificationService = inject(NotificationService);

  blockedPanel: boolean = false;
  items: ProductDto[] = [];

  public skipCount: number = 0;
  public maxResultCount: number = 10;
  public totalCount: number | undefined = 0;

  // Filters
  productCategories: any[] = [];
  keyword: string = '';
  categoryId: string = '';

  visibleForm = false;
  productId: string = '';
  headerTitle = '';

  ngOnInit(): void {
    this.loadData();
    this.loadProductCategories();
  }

  loadData() {
    this.toggleBlockUI(true);
    this.productService
      .getListFilter({
        keyword: this.keyword,
        categoryId: this.categoryId,
        maxResultCount: this.maxResultCount,
        skipCount: this.skipCount,
      })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: PagedResultDto<ProductDto>) => {
          this.items = response.items ?? [];
          this.totalCount = response.totalCount;
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  loadProductCategories() {
    this.productCategoryService.getListAll().subscribe((response: ProductCategoryDto[]) => {
      console.log(response);
      response.forEach(e => {
        this.productCategories.push({
          value: e.id,
          label: e.name,
        });
      });
    });
  }

  pageChanged(event: any): void {
    this.skipCount = (event.page - 1) * this.maxResultCount;
    this.maxResultCount = event.rows;
    this.loadData();
  }

  showAddModal() {
    this.headerTitle = 'Thêm mới sản phẩm';
    this.visibleForm = true;
    this.productId = '';
  }

  showEditModal(id: string) {
    this.headerTitle = 'Chỉnh sửa sản phẩm';
    this.visibleForm = true;
    this.productId = id;
  }

  saveData() {
    this.visibleForm = false;
    this.loadData();
    this.notificationService.showSuccess('Thêm sản phẩm thành công');
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
