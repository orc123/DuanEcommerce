import { Component, EventEmitter, inject, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { forkJoin, Subject, takeUntil } from 'rxjs';
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
import { CheckboxModule } from 'primeng/checkbox';
import { QuillEditorComponent } from 'ngx-quill';
import { ValidationMessage } from '../shared/validation-message';
import { DropdownModule } from 'primeng/dropdown';
import { ManufacturerDto, ManufacturersService } from '../proxy/manufacturers';
import { productTypeOptions } from '../proxy/duan-ecommerce/products';
import { UtilityService } from '../shared/services/utility.service';

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
    CheckboxModule,
    QuillEditorComponent,
    ValidationMessage,
    DropdownModule,
  ],
  template: `
    @if (form) {
      <form [formGroup]="form" skipValidation (ngSubmit)="saveChanges()">
        <p-panel #pnl header="Sản phẩm">
          <div class="formgrid grid">
            <div class="field col-12">
              <label for="name" class="block">Tên <span class="required">*</span></label>
              <input
                id="name"
                pInputText
                type="text"
                class="w-full"
                formControlName="name"
                (keyup)="generateSlug()"
              />
              <app-validation-message
                [entityForm]="form"
                fieldName="name"
                [validationMessages]="validationMessages"
              ></app-validation-message>
            </div>
            <div class="field col-12">
              <label for="code" class="block">Code <span class="required">*</span></label>
              <input id="code" pInputText type="text" class="w-full" formControlName="code" />
              <app-validation-message
                [entityForm]="form"
                fieldName="code"
                [validationMessages]="validationMessages"
              ></app-validation-message>
            </div>
            <div class="field col-12">
              <label for="slug" class="block">Slug <span class="required">*</span></label>
              <input id="slug" pInputText type="text" class="w-full" formControlName="slug" />
              <app-validation-message
                [entityForm]="form"
                fieldName="slug"
                [validationMessages]="validationMessages"
              ></app-validation-message>
            </div>
            <div class="field col-12">
              <label for="sku" class="block">SKU <span class="required">*</span></label>
              <input id="sku" pInputText type="text" class="w-full" formControlName="sku" />
              <app-validation-message
                [entityForm]="form"
                fieldName="sku"
                [validationMessages]="validationMessages"
              ></app-validation-message>
            </div>

            <div class="field col-12">
              <label for="manufacturerId" class="block"
                >Nhà sản xuất <span class="required">*</span></label
              >
              <p-dropdown
                [options]="manufactures"
                formControlName="manufacturerId"
                placeholder="Chọn nhà sản xuất"
                [showClear]="true"
                [style]="{ width: '100%' }"
                optionValue="value"
                optionLabel="label"
              ></p-dropdown>
              <app-validation-message
                [entityForm]="form"
                fieldName="manufacturerId"
                [validationMessages]="validationMessages"
              ></app-validation-message>
            </div>
            <div class="field col-12">
              <label for="categoryId" class="block">Danh mục <span class="required">*</span></label>
              <p-dropdown
                [options]="productCategories"
                formControlName="categoryId"
                placeholder="Chọn danh mục"
                [showClear]="true"
                [style]="{ width: '100%' }"
                optionValue="value"
                optionLabel="label"
              ></p-dropdown>
              <app-validation-message
                [entityForm]="form"
                fieldName="categoryId"
                [validationMessages]="validationMessages"
              ></app-validation-message>
            </div>
            <div class="field col-12">
              <label for="productType" class="block"
                >Loại sản phẩm <span class="required">*</span></label
              >
              <p-dropdown
                [options]="productTypes"
                formControlName="productType"
                placeholder="Chọn loại"
                [showClear]="true"
                [style]="{ width: '100%' }"
                optionValue="value"
                optionLabel="label"
              ></p-dropdown>
              <app-validation-message
                [entityForm]="form"
                fieldName="productType"
                [validationMessages]="validationMessages"
              ></app-validation-message>
            </div>

            <div class="field col-12">
              <label for="sortOrder" class="block">Thứ tự <span class="required">*</span></label>
              <p-input-number
                id="sortOrder"
                class="w-full"
                [style]="{ width: '100%' }"
                formControlName="sortOrder"
              ></p-input-number>
              <app-validation-message
                [entityForm]="form"
                fieldName="sortOrder"
                [validationMessages]="validationMessages"
              ></app-validation-message>
            </div>
            <div class="field col-12">
              <label for="sellPrice" class="block">Giá bán <span class="required">*</span></label>
              <p-input-number
                id="sellPrice"
                class="w-full"
                [style]="{ width: '100%' }"
                formControlName="sellPrice"
              ></p-input-number>
              <app-validation-message
                [entityForm]="form"
                fieldName="sellPrice"
                [validationMessages]="validationMessages"
              ></app-validation-message>
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
            <button
              type="submit"
              [disabled]="!form.valid || btnDisabled"
              pButton
              icon="fa fa-save"
              label="Lưu lại"
            ></button>
          </ng-template>
          <p-block-ui [blocked]="blockedPanel" [target]="pnl">
            <p-progressSpinner></p-progressSpinner>
          </p-block-ui>
        </p-panel>
      </form>
    }
  `,
})
export class ProductDetail implements OnInit, OnDestroy {
  fb = inject(FormBuilder);
  productCategoryService = inject(ProductCategoriesService);
  productService = inject(ProductsService);
  manufacturerService = inject(ManufacturersService);
  utilityService = inject(UtilityService);

  private ngUnsubscribe = new Subject<void>();
  public form!: FormGroup;

  selectedEntity = {} as ProductDto;

  productCategories: any[] = [];
  manufactures: any[] = [];
  productTypes: any[] = [];
  @Input() productId: string = '';
  @Output() saveChange = new EventEmitter<void>();

  btnDisabled = false;

  validationMessages = {
    code: [{ type: 'required', message: 'Bạn phải nhập mã duy nhất' }],
    name: [
      { type: 'required', message: 'Bạn phải nhập tên' },
      { type: 'maxlength', message: 'Bạn không được nhập quá 255 kí tự' },
    ],
    slug: [{ type: 'required', message: 'Bạn phải URL duy nhất' }],
    sku: [{ type: 'required', message: 'Bạn phải mã SKU sản phẩm' }],
    manufacturerId: [{ type: 'required', message: 'Bạn phải chọn nhà cung cấp' }],
    categoryId: [{ type: 'required', message: 'Bạn phải chọn danh mục' }],
    productType: [{ type: 'required', message: 'Bạn phải chọn loại sản phẩm' }],
    sortOrder: [{ type: 'required', message: 'Bạn phải nhập thứ tự' }],
    sellPrice: [{ type: 'required', message: 'Bạn phải nhập giá bán' }],
  };

  blockedPanel: boolean = false;
  ngOnInit(): void {
    this.selectedEntity = {};
    this.loadProductTypes();
    var productCategories = this.productCategoryService.getListAll();
    var manufactures = this.manufacturerService.getListAll();
    this.toggleBlockUI(true);
    forkJoin({
      productCategories,
      manufactures,
    })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: any) => {
          this.productCategories = [];
          this.manufactures = [];
          var productCategoriesList = response.productCategories as ProductCategoryDto[];
          var manufacturesList = response.manufactures as ManufacturerDto[];
          productCategoriesList.forEach(e => {
            this.productCategories.push({
              value: e.id,
              label: e.name,
            });
          });
          manufacturesList.forEach(e => {
            this.manufactures.push({
              value: e.id,
              label: e.name,
            });
          });

          if (this.utilityService.isEmpty(this.productId)) {
            this.buildForm();
            this.toggleBlockUI(false);
          } else {
            this.loadFormDetails(this.productId);
          }
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  generateSlug() {
    const name = this.form?.get('name')?.value;
    if (name) {
      this.form.controls['slug'].setValue(this.utilityService.MakeSeoTitle(name));
    }
  }

  buildForm() {
    this.form = this.fb.group({
      name: new FormControl(
        this.selectedEntity.name || null,
        Validators.compose([Validators.required, Validators.maxLength(255)]),
      ),
      code: new FormControl(
        this.selectedEntity.code || null,
        Validators.compose([Validators.required, Validators.maxLength(50)]),
      ),
      slug: new FormControl(
        this.selectedEntity.slug || null,
        Validators.compose([Validators.required, Validators.maxLength(255)]),
      ),
      sku: new FormControl(
        this.selectedEntity.sku || null,
        Validators.compose([Validators.required, Validators.maxLength(50)]),
      ),
      manufacturerId: new FormControl(
        this.selectedEntity.manufacturerId || null,
        Validators.required,
      ),
      categoryId: new FormControl(this.selectedEntity.categoryId || null, Validators.required),
      productType: new FormControl(this.selectedEntity.productType ?? null, Validators.required),
      sortOrder: new FormControl(this.selectedEntity.sortOrder ?? null, Validators.required),
      sellPrice: new FormControl(this.selectedEntity.sellPrice ?? null, Validators.required),
      visibility: new FormControl(this.selectedEntity.visibility ?? true),
      isActive: new FormControl(this.selectedEntity.isActive ?? true),
      seoMetaDescription: new FormControl(this.selectedEntity.seoMetaDescription || null),
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

  loadProductTypes() {
    productTypeOptions.forEach(e => {
      this.productTypes.push({
        value: e.value,
        label: e.key,
      });
    });
  }

  saveChanges() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.toggleBlockUI(true);
    const saveObservable = this.utilityService.isEmpty(this.productId)
      ? this.productService.create(this.form.value)
      : this.productService.update(this.productId!, this.form.value);

    saveObservable.pipe(takeUntil(this.ngUnsubscribe)).subscribe({
      next: () => {
        this.toggleBlockUI(false);
        this.saveChange.emit();
      },
      error: () => {
        this.toggleBlockUI(false);
      },
    });
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  private toggleBlockUI(enabled: boolean) {
    if (enabled) {
      this.blockedPanel = true;
      this.btnDisabled = true;
    } else {
      setTimeout(() => {
        this.blockedPanel = false;
        this.btnDisabled = false;
      }, 300);
    }
  }
}
