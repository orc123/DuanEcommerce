import { Component, EventEmitter, inject, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Panel, PanelModule } from 'primeng/panel';
import { BlockUI } from 'primeng/blockui';
import { ProgressSpinner } from 'primeng/progressspinner';
import { InputText } from 'primeng/inputtext';
import { ButtonDirective } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { ValidationMessage } from '../shared/validation-message';
import { DropdownModule } from 'primeng/dropdown';
import { attributeTypeOptions } from '../proxy/duan-ecommerce/product-attributes';
import { UtilityService } from '../shared/services/utility.service';
import { TextareaModule } from 'primeng/textarea';
import { NotificationService } from '../shared/services/notification.service';
import { ImageModule } from 'primeng/image';
import { ProductAttributeDto, ProductAttributesService } from '../proxy/product-attributes';

@Component({
  selector: 'app-attribute-detail',
  imports: [
    ReactiveFormsModule,
    Panel,
    BlockUI,
    ProgressSpinner,
    InputText,
    PanelModule,
    ButtonDirective,
    CheckboxModule,
    ValidationMessage,
    DropdownModule,
    TextareaModule,
    ImageModule,
  ],
  template: `
    @if (form) {
      <form [formGroup]="form" skipValidation (ngSubmit)="saveChanges()">
        <p-panel #pnl header="Chi tiết Thuộc tính Sản phẩm">
          <div class="formgrid grid">
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
              <label for="label" class="block">Nhãn <span class="required">*</span></label>
              <input id="label" pInputText type="text" class="w-full" formControlName="label" />
              <app-validation-message
                [entityForm]="form"
                fieldName="label"
                [validationMessages]="validationMessages"
              ></app-validation-message>
            </div>
            <div class="field col-12">
              <label for="dataType" class="block"
                >Kiểu dữ liệu <span class="required">*</span></label
              >
              <p-dropdown
                [options]="dataTypes"
                formControlName="dataType"
                placeholder="Chọn kiểu dữ liệu"
                [showClear]="true"
                [style]="{ width: '100%' }"
                optionValue="value"
                optionLabel="label"
              ></p-dropdown>
              <app-validation-message
                [entityForm]="form"
                fieldName="dataType"
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
            <div class="field-checkbox col-12 md:col-3">
              <p-checkbox formControlName="isRequired" binary="true" id="isRequired"></p-checkbox>
              <label for="isRequired">Bắt buộc nhập</label>
            </div>
            <div class="field-checkbox col-12 md:col-3">
              <p-checkbox formControlName="isUnique" binary="true" id="isUnique"></p-checkbox>
              <label for="isUnique">Duy nhất</label>
            </div>
            <div class="field col-12">
              <label for="note" class="block">Ghi chú</label>
              <textarea id="note" pInputTextarea class="w-full" formControlName="note"></textarea>
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
export class AttributeDetail implements OnInit, OnDestroy {
  fb = inject(FormBuilder);
  productAttributesService = inject(ProductAttributesService);
  utilityService = inject(UtilityService);
  notificationService = inject(NotificationService);

  private ngUnsubscribe = new Subject<void>();
  public form!: FormGroup;

  selectedEntity = {} as ProductAttributeDto;

  dataTypes: any[] = [];
  public thumbnailImage: any;
  @Input() attributeId: string = '';
  @Output() saveChange = new EventEmitter<void>();

  btnDisabled = false;

  validationMessages = {
    code: [{ type: 'required', message: 'Bạn phải nhập mã duy nhất' }],
    label: [
      { type: 'required', message: 'Bạn phải nhập Label' },
      { type: 'maxlength', message: 'Bạn không được nhập quá 50 kí tự' },
    ],
    dataType: [{ type: 'required', message: 'Bạn phải chọn kiểu dữ liệu' }],
  };

  blockedPanel: boolean = false;
  ngOnInit(): void {
    this.selectedEntity = {};
    this.loadDataTypes();
    this.initFormData();
  }

  initFormData() {
    this.toggleBlockUI(true);
    if (this.utilityService.isEmpty(this.attributeId)) {
      this.buildForm();
      this.toggleBlockUI(false);
    } else {
      this.loadFormDetails(this.attributeId);
      this.toggleBlockUI(false);
    }
  }

  buildForm() {
    this.form = this.fb.group({
      code: new FormControl(
        this.selectedEntity.code || null,
        Validators.compose([Validators.required, Validators.maxLength(50)]),
      ),
      label: new FormControl(
        this.selectedEntity.label || null,
        Validators.compose([Validators.required, Validators.maxLength(50)]),
      ),
      dataType: new FormControl(this.selectedEntity.dataType ?? null, Validators.required),
      visibility: new FormControl(this.selectedEntity.visibility ?? true),
      isActive: new FormControl(this.selectedEntity.isActive ?? true),
      isRequired: new FormControl(this.selectedEntity.isRequired ?? true),
      isUnique: new FormControl(this.selectedEntity.isUnique ?? true),
      note: new FormControl(this.selectedEntity.note || null),
    });
  }

  loadFormDetails(id: string) {
    this.toggleBlockUI(true);

    this.productAttributesService
      .get(id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (res: ProductAttributeDto) => {
          this.selectedEntity = res;
          this.buildForm();
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  loadDataTypes() {
    attributeTypeOptions.forEach(e => {
      this.dataTypes.push({
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
    var message = this.utilityService.isEmpty(this.attributeId)
      ? 'Thêm thuộc tính thành công'
      : 'Cập nhật thuộc tính thành công';
    this.toggleBlockUI(true);
    const saveObservable = this.utilityService.isEmpty(this.attributeId)
      ? this.productAttributesService.create(this.form.value)
      : this.productAttributesService.update(this.attributeId, this.form.value);

    saveObservable.pipe(takeUntil(this.ngUnsubscribe)).subscribe({
      next: () => {
        this.toggleBlockUI(false);
        this.saveChange.emit();

        this.notificationService.showSuccess(message);
      },
      error: err => {
        this.notificationService.showError(err.error.message);
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
