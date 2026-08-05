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
import { ValidationMessage } from '../shared/validation-message';
import { UtilityService } from '../shared/services/utility.service';
import { NotificationService } from '../shared/services/notification.service';
import { RoleDto, RolesService } from '../proxy/roles';
import { MessageConstants } from '../shared/constants/message.const';

@Component({
  selector: 'app-role-detail',
  imports: [
    ReactiveFormsModule,
    Panel,
    BlockUI,
    ProgressSpinner,
    InputText,
    PanelModule,
    ValidationMessage,
  ],
  template: `
    @if (form) {
      <form
        class="form-horizontal form-label-left"
        skipValidation
        [formGroup]="form"
        (ngSubmit)="saveChanges()"
      >
        <p-panel #pnlDetail header="Thông tin" [toggleable]="true" [collapsed]="false">
          <div class="formgrid grid">
            <div class="field col-12 md:col-6">
              <label for="name">Tên <span class="required">*</span></label>
              <input
                id="name"
                type="text"
                pInputText
                formControlName="name"
                [style]="{ width: '100%' }"
              />
              <app-validation-message
                [entityForm]="form"
                fieldName="name"
                [validationMessages]="validationMessages"
              ></app-validation-message>
            </div>
            <div class="field col-12 md:col-6">
              <label for="description">Mô tả <span class="required">*</span></label>
              <input
                id="description"
                type="text"
                pInputText
                formControlName="description"
                [style]="{ width: '100%' }"
              />
              <app-validation-message
                [entityForm]="form"
                fieldName="description"
                [validationMessages]="validationMessages"
              ></app-validation-message>
            </div>
          </div>
          <ng-template pTemplate="footer">
            <button type="submit" class="btn btn-primary" [disabled]="!form.valid || btnDisabled">
              {{ saveBtnName }}
            </button>
          </ng-template>

          <p-blockUI [target]="pnlDetail" [blocked]="blockedPanel">
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
      </form>
    }
  `,
})
export class RoleDetail implements OnInit, OnDestroy {
  fb = inject(FormBuilder);
  rolesService = inject(RolesService);
  utilityService = inject(UtilityService);
  notificationService = inject(NotificationService);

  private ngUnsubscribe = new Subject<void>();
  public form!: FormGroup;

  selectedEntity = {} as RoleDto;

  dataTypes: any[] = [];
  public thumbnailImage: any;
  @Input() roleId: string = '';
  @Output() saveChange = new EventEmitter<void>();
  public saveBtnName!: string;
  public closeBtnName!: string;

  btnDisabled = false;

  validationMessages = {
    name: [
      { type: 'required', message: 'Bạn phải nhập tên nhóm' },
      { type: 'minlength', message: 'Bạn phải nhập ít nhất 3 kí tự' },
      { type: 'maxlength', message: 'Bạn không được nhập quá 255 kí tự' },
    ],
    description: [{ type: 'required', message: 'Bạn phải nhập Mô tả' }],
  };

  blockedPanel: boolean = false;
  ngOnInit(): void {
    this.selectedEntity = {};
    this.initFormData();
  }

  initFormData() {
    this.toggleBlockUI(true);
    if (this.utilityService.isEmpty(this.roleId)) {
      this.buildForm();

      this.saveBtnName = 'Thêm';
      this.closeBtnName = 'Đóng';
      this.toggleBlockUI(false);
    } else {
      this.saveBtnName = 'Cập nhật';
      this.closeBtnName = 'Hủy';
      this.loadFormDetails(this.roleId);
      this.toggleBlockUI(false);
    }
  }

  buildForm() {
    this.form = this.fb.group({
      name: new FormControl(
        this.selectedEntity.name || null,
        Validators.compose([
          Validators.required,
          Validators.maxLength(255),
          Validators.minLength(3),
        ]),
      ),
      description: new FormControl(this.selectedEntity.description || null, Validators.required),
    });
  }

  loadFormDetails(id: string) {
    this.toggleBlockUI(true);
    this.rolesService
      .get(id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (res: RoleDto) => {
          this.selectedEntity = res;
          this.buildForm();
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  saveChanges() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    var message = this.utilityService.isEmpty(this.roleId)
      ? MessageConstants.CREATED_OK_MSG
      : MessageConstants.UPDATED_OK_MSG;
    this.toggleBlockUI(true);
    const saveObservable = this.utilityService.isEmpty(this.roleId)
      ? this.rolesService.create(this.form.value)
      : this.rolesService.update(this.roleId, this.form.value);

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
