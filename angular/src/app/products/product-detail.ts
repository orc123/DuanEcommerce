import { Component, EventEmitter, inject, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { ProductCategoriesService, ProductCategoryDto } from '../proxy/product-categories';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ProductDto, ProductsService } from '../proxy/products';
import { Panel, PanelModule } from 'primeng/panel';
import { BlockUI } from 'primeng/blockui';
import { ProgressSpinner } from 'primeng/progressspinner';
import { InputText } from 'primeng/inputtext';
import { ButtonDirective } from 'primeng/button';
import { InputNumber } from 'primeng/inputnumber';
import { DropdownModule } from 'primeng/dropdown';
import { CheckboxModule } from 'primeng/checkbox';
import { QuillEditorComponent } from 'ngx-quill';

@Component({
  selector: 'app-product-detail',
  imports: [
    ReactiveFormsModule,
    Panel,
    BlockUI,
    ProgressSpinner,
    InputText,
    PanelModule,
    ButtonDirective,
    InputNumber,
    DropdownModule,
    CheckboxModule,
    QuillEditorComponent,
  ],
  template: `
    <form [formGroup]="form" (ngSubmit)="saveChanges()">
      <p-panel #pnl header="Sản phẩm">
        <div class="formgrid grid">
          <div class="field col-12">
            <label for="name" class="block">Tên</label>
            <input id="name" pInputText type="text" class="w-full" formControlName="name" />
          </div>
          <div class="field col-12">
            <label for="code" class="block">Code</label>
            <input id="code" pInputText type="text" class="w-full" formControlName="code" />
          </div>
          <div class="field col-12">
            <label for="slug" class="block">Slug</label>
            <input id="slug" pInputText type="text" class="w-full" formControlName="slug" />
          </div>
          <div class="field col-12">
            <label for="sku" class="block">SKU</label>
            <input id="sku" pInputText type="text" class="w-full" formControlName="sku" />
          </div>

          <div class="field col-12">
            <label for="manufacturerId" class="block">Nhà sản xuất</label>
            <p-dropdown
              [options]="manufactures"
              formControlName="manufacturerId"
              placeholder="Chọn nhà sản xuất"
              [showClear]="true"
              autoWidth="false"
              [style]="{ width: '100%' }"
            ></p-dropdown>
          </div>
          <div class="field col-12">
            <label for="categoryId" class="block">Danh mục</label>
            <p-dropdown
              [options]="productCategories"
              formControlName="categoryId"
              placeholder="Chọn danh mục"
              [showClear]="true"
              autoWidth="false"
              [style]="{ width: '100%' }"
            ></p-dropdown>
          </div>
          <div class="field col-12">
            <label for="productType" class="block">Loại sản phẩm</label>
            <p-dropdown
              [options]="productTypes"
              formControlName="productType"
              placeholder="Chọn loại"
              [showClear]="true"
              autoWidth="false"
              [style]="{ width: '100%' }"
            ></p-dropdown>
          </div>

          <div class="field col-12">
            <label for="sortOrder" class="block">Thứ tự</label>
            <p-input-number
              id="sortOrder"
              pInputText
              type="text"
              class="w-full"
              formControlName="sortOrder"
            />
          </div>
          <div class="field col-12">
            <label for="sellPrice" class="block">Giá bán</label>
            <p-input-number
              id="sortOrder"
              pInputText
              type="text"
              class="w-full"
              formControlName="sellPrice"
            />
          </div>
          <div class="field-checkbox col-12 md:col-3">
            <p-checkbox formControlName="visibility" binary="true" id="visibility"></p-checkbox>
            <label for="visibility">Hiển thị</label>
          </div>
          <div class="field-checkbox col-12 md:col-3">
            <p-checkbox formControlName="isActive" binary="true" id="isActive"></p-checkbox>
            <label for="isActive">Kích hoạt</label>
          </div>

          <div class="field col-12">
            <label for="seoMetaDescription" class="block">Mô tả SEO</label>
            <textarea
              id="seoMetaDescription"
              pInputTextarea
              class="w-full"
              formControlName="seoMetaDescription"
            ></textarea>
          </div>
          <div class="field col-12">
            <label for="description" class="block">Mô tả</label>
            <!-- <textarea
              pInputTextarea
              formControlName="description"
              [style]="{ height: '120px' }"
              class="w-full"
            ></textarea> -->
            <div class="editor-container">
              <quill-editor
                formControlName="description"
                [style]="{ height: '320px' }"
                class="w-full"
              ></quill-editor>
            </div>
          </div>
        </div>
        <ng-template pTemplate="footer">
          <button type="submit" pButton icon="fa fa-save" label="Lưu lại"></button>
        </ng-template>
        <p-block-ui [blocked]="blockedPanel" [target]="pnl">
          <p-progressSpinner></p-progressSpinner>
        </p-block-ui>
      </p-panel>
    </form>
  `,
})
export class ProductDetail implements OnInit, OnDestroy {
  fb = inject(FormBuilder);
  productCategoryService = inject(ProductCategoriesService);
  productService = inject(ProductsService);

  private ngUnsubscribe = new Subject<void>();
  public form!: FormGroup;

  selectedEntity = {} as ProductDto;

  productCategories: any[] = [];
  manufactures: any[] = [];
  productTypes: any[] = [];
  @Input() product: ProductDto = {};
  @Output() saveChange = new EventEmitter<void>();

  blockedPanel: boolean = false;
  ngOnInit(): void {
    this.selectedEntity = this.product;
    this.buildForm();
  }

  buildForm() {
    this.form = this.fb.group({
      name: new FormControl(this.selectedEntity.name || null, Validators.required),
      code: new FormControl(this.selectedEntity.code || null, Validators.required),
      slug: new FormControl(this.selectedEntity.slug || null, Validators.required),
      sku: new FormControl(this.selectedEntity.sku || null, Validators.required),
      manufacturerId: new FormControl(
        this.selectedEntity.manufacturerId || null,
        Validators.required,
      ),
      categoryId: new FormControl(this.selectedEntity.categoryId || null, Validators.required),
      productType: new FormControl(this.selectedEntity.productType || null, Validators.required),
      sortOrder: new FormControl(this.selectedEntity.sortOrder || null, Validators.required),
      sellPrice: new FormControl(this.selectedEntity.sellPrice || null, Validators.required),
      visibility: new FormControl(this.selectedEntity.visibility || false),

      isActive: new FormControl(this.selectedEntity.isActive || false),
      seoMetaDescription: new FormControl(
        this.selectedEntity.seoMetaDescription || null,
        Validators.required,
      ),
      description: new FormControl(this.selectedEntity.description || null),
    });
  }

  loadFormDetails(id: string) {
    this.toggleBlockUI(true);

    this.productService
      .get(id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (res: ProductDto) => {
          this.selectedEntity = res;
          this.buildForm();
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
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
      console.log(this.productCategories);
    });
  }

  saveChanges() {
    this.saveChange.emit();
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
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
}
