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

@Component({
  selector: 'app-product',
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
  ],
  template: `
    <p-panel header="Danh sách sản phẩm">
      <div class="grid">
        <div class="col-4">
          <button pButton icon="fa fa-plus" label="Thêm mới"></button>
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
      <p-block-ui [blocked]="blockedPanel" [target]="pnl"></p-block-ui>
    </p-panel>
  `,
})
export class Product implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();

  productService = inject(ProductsService);
  productCategoryService = inject(ProductCategoriesService);

  blockedPanel: boolean = false;
  items: ProductDto[] = [];

  public skipCount: number = 0;
  public maxResultCount: number = 10;
  public totalCount: number | undefined = 0;

  // Filters
  productCategories: any[] = [];
  keyword: string = '';
  categoryId: string = '';

  ngOnInit(): void {
    this.loadData();
    this.loadProductCategories();
  }

  loadData() {
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
        },
        error: () => {},
      });
  }

  loadProductCategories() {
    this.productCategoryService.getListAll().subscribe((response: ProductCategoryDto[]) => {
      response.forEach(e => {
        this.productCategories.push({
          value: e.id,
          name: e.name,
        });
      });
    });
  }

  pageChanged(event: any): void {
    this.skipCount = (event.page - 1) * this.maxResultCount;
    this.maxResultCount = event.rows;
    this.loadData();
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
}
