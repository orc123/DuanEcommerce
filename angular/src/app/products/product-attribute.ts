import { Component, EventEmitter, inject, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ProductsService } from '../proxy/products';
import { PanelModule } from 'primeng/panel';
import { BlockUI } from 'primeng/blockui';
import { ProgressSpinner } from 'primeng/progressspinner';
import { InputText } from 'primeng/inputtext';
import { ButtonDirective } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { DropdownModule } from 'primeng/dropdown';
import { TextareaModule } from 'primeng/textarea';
import { NotificationService } from '../shared/services/notification.service';
import { ImageModule } from 'primeng/image';
import { ProductAttributeDto, ProductAttributesService } from '../proxy/product-attributes';
import { ConfirmationService } from 'primeng/api';
import { AttributeType } from '../proxy/duan-ecommerce/product-attributes';
import { ProductAttributeValueDto } from '../proxy/products/attributes';
import { TableModule } from 'primeng/table';
import { InputNumberModule } from 'primeng/inputnumber'; // 👈 Import cái này
import { CalendarModule } from 'primeng/calendar'; // 👈 Import cái này nếu có dùng p-calendar
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-product-attribute',
  imports: [
    ReactiveFormsModule,
    BlockUI,
    ProgressSpinner,
    InputText,
    PanelModule,
    ButtonDirective,
    CheckboxModule,
    DropdownModule,
    TextareaModule,
    ImageModule,
    TableModule,
    InputNumberModule,
    CalendarModule,
  ],
  template: `
    @if (form) {
      <form [formGroup]="form" skipValidation (ngSubmit)="saveChanges()">
        <div class="grid">
          <div class="col-3">
            <div class="field">
              <p-dropdown
                [options]="attributes"
                [style]="{ width: '100%' }"
                placeholder="Chọn thuộc tính"
                [showClear]="true"
                formControlName="attributeId"
                optionValue="value"
                optionLabel="label"
                (onChange)="selectAttribute($event)"
              >
              </p-dropdown>
            </div>
          </div>
          <div class="col-3">
            <div class="field">
              <p-calendar
                formControlName="dateTimeValue"
                [hidden]="!showDateTimeControl"
              ></p-calendar>
              <p-inputNumber formControlName="intValue" [hidden]="!showIntControl"></p-inputNumber>
              <p-inputNumber
                formControlName="decimalValue"
                mode="decimal"
                [hidden]="!showDecimalControl"
              ></p-inputNumber>
              <input
                type="text"
                pInputText
                formControlName="varcharValue"
                [hidden]="!showVarcharControl"
                class="w-full"
              />
              <input
                type="text"
                pInputText
                formControlName="textValue"
                [hidden]="!showTextControl"
                class="w-full"
              />
            </div>
          </div>
          <div class="col-3">
            <button type="button" (click)="saveChanges()" [disabled]="!form.valid" pButton>
              Thêm
            </button>
          </div>
        </div>
      </form>
      <!--Table-->
      <p-table #tableCourses [value]="productAttributes" dataKey="id">
        <ng-template pTemplate="header">
          <tr>
            <th>Mã</th>
            <th style="width: 20%">Tên thuộc tính</th>
            <th style="width: 20%">Kiểu dữ liệu</th>
            <th>Giá trị</th>
            <th></th>
          </tr>
        </ng-template>
        <ng-template pTemplate="body" let-row>
          <tr>
            <td>
              {{ row.label }}
            </td>
            <td>{{ getDataTypeName(row.dataType) }}</td>
            <td>{{ getValueByType(row, row.dataType) }}</td>
            <td>
              <button
                pButton
                pRipple
                (click)="removeItem(row)"
                type="button"
                icon="pi pi-times"
                class="p-button-rounded p-button-danger p-button-text"
              ></button>
            </td>
          </tr>
        </ng-template>
      </p-table>
      <!--Block UI-->
      <p-blockUI [blocked]="blockedPanel">
        <p-progressSpinner></p-progressSpinner>
      </p-blockUI>
    }
  `,
  providers: [DatePipe],
})
export class ProductAttribute implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  blockedPanel: boolean = false;
  btnDisabled = false;
  public form!: FormGroup;

  attributes: any[] = [];
  fullAttributes: any[] = [];

  productAttributes: any[] = [];

  showDateTimeControl: boolean = false;
  showDecimalControl: boolean = false;
  showIntControl: boolean = false;
  showVarcharControl: boolean = false;
  showTextControl: boolean = false;

  @Input() productId: string = '';

  productAttributeService = inject(ProductAttributesService);
  productService = inject(ProductsService);
  fb = inject(FormBuilder);
  notificationService = inject(NotificationService);
  confirmationService = inject(ConfirmationService);
  private datePipe = inject(DatePipe);

  ngOnInit() {
    this.buildForm();
    this.initFormData();
  }

  buildForm() {
    this.form = this.fb.group({
      productId: new FormControl(this.productId),
      attributeId: new FormControl(null, Validators.required),
      dateTimeValue: new FormControl(null),
      decimalValue: new FormControl(null),
      intValue: new FormControl(null),
      varcharValue: new FormControl(null),
      textValue: new FormControl(null),
    });
  }

  initFormData() {
    var attributes = this.productAttributeService.getListAll();
    this.toggleBlockUI(true);
    forkJoin({
      attributes,
    })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: res => {
          this.fullAttributes = res.attributes;
          var attributeOptions = res.attributes as ProductAttributeDto[];
          attributeOptions.forEach(element => {
            this.attributes.push({
              label: element.label,
              value: element.id,
            });
          });
          this.loadFormDetails(this.productId);
        },
        error: err => {
          this.toggleBlockUI(false);
        },
      });
  }

  loadFormDetails(productId: string) {
    this.toggleBlockUI(true);
    this.productService
      .getProductAttributeAll(productId)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: res => {
          this.productAttributes = res;
          this.buildForm();
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  selectAttribute(event: any) {
    var dataType = this.fullAttributes.filter(x => x.id == event.value)[0].dataType;
    this.showDateTimeControl = false;
    this.showDecimalControl = false;
    this.showIntControl = false;
    this.showTextControl = false;
    this.showVarcharControl = false;
    if (dataType == AttributeType.Date) {
      this.showDateTimeControl = true;
    } else if (dataType == AttributeType.Decimal) {
      this.showDecimalControl = true;
    } else if (dataType == AttributeType.Int) {
      this.showIntControl = true;
    } else if (dataType == AttributeType.Text) {
      this.showTextControl = true;
    } else if (dataType == AttributeType.Varchar) {
      this.showVarcharControl = true;
    }
  }

  saveChanges() {
    this.toggleBlockUI(true);
    var selectedAttributeId = this.form.controls['attributeId'].value;
    var dataType = this.fullAttributes.filter(x => x.id == selectedAttributeId)[0].dataType;
    if (dataType == AttributeType.Date) {
      this.form.controls['decimalValue'].setValue(null);
      this.form.controls['intValue'].setValue(null);
      this.form.controls['textValue'].setValue(null);
      this.form.controls['varcharValue'].setValue(null);
    } else if (dataType == AttributeType.Decimal) {
      this.form.controls['dateTimeValue'].setValue(null);
      this.form.controls['intValue'].setValue(null);
      this.form.controls['textValue'].setValue(null);
      this.form.controls['varcharValue'].setValue(null);
    } else if (dataType == AttributeType.Int) {
      this.form.controls['dateTimeValue'].setValue(null);
      this.form.controls['decimalValue'].setValue(null);
      this.form.controls['textValue'].setValue(null);
      this.form.controls['varcharValue'].setValue(null);
    } else if (dataType == AttributeType.Text) {
      this.form.controls['dateTimeValue'].setValue(null);
      this.form.controls['decimalValue'].setValue(null);
      this.form.controls['intValue'].setValue(null);
      this.form.controls['varcharValue'].setValue(null);
    } else if (dataType == AttributeType.Varchar) {
      this.form.controls['dateTimeValue'].setValue(null);
      this.form.controls['decimalValue'].setValue(null);
      this.form.controls['intValue'].setValue(null);
      this.form.controls['textValue'].setValue(null);
    }
    this.productService
      .addAttribute(this.form.value)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: () => {
          this.toggleBlockUI(false);
          this.loadFormDetails(this.productId);
        },
        error: err => {
          this.notificationService.showError(err.error.error.message);
          this.toggleBlockUI(false);
        },
      });
  }

  removeItem(attribute: ProductAttributeValueDto) {
    var id = '';
    if (attribute.dataType == AttributeType.Date) {
      id = attribute.dateTimeId!;
    } else if (attribute.dataType == AttributeType.Decimal) {
      id = attribute.decimalId!;
    } else if (attribute.dataType == AttributeType.Int) {
      id = attribute.intId!;
    } else if (attribute.dataType == AttributeType.Text) {
      id = attribute.textId!;
    } else if (attribute.dataType == AttributeType.Varchar) {
      id = attribute.varcharId!;
    }
    this.confirmationService.confirm({
      message: 'Bạn có chắc muốn xóa bản ghi này?',
      accept: () => {
        this.deleteItemsConfirmed(attribute.attributeId!, id);
      },
    });
  }
  deleteItemsConfirmed(attribbuteId: string, id: string) {
    this.toggleBlockUI(true);
    this.productService
      .removeProductAttribute(attribbuteId, id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: () => {
          this.notificationService.showSuccess('Xóa thành công');
          this.loadFormDetails(this.productId);
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }
  getDataTypeName(value: number) {
    return AttributeType[value];
  }
  getValueByType(attribute: ProductAttributeValueDto, value: number) {
    if (attribute.dataType == AttributeType.Date) {
      return this.datePipe.transform(attribute.dateTimeValue, 'dd/MM/yyyy');
    } else if (attribute.dataType == AttributeType.Decimal) {
      return attribute.decimalValue;
    } else if (attribute.dataType == AttributeType.Int) {
      return attribute.intValue;
    } else if (attribute.dataType == AttributeType.Text) {
      return attribute.textValue;
    } else if (attribute.dataType == AttributeType.Varchar) {
      return attribute.varcharValue;
    }
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
