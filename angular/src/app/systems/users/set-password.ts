import { Component, EventEmitter, inject, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { Panel, PanelModule } from 'primeng/panel';
import { BlockUI } from 'primeng/blockui';
import { ProgressSpinner } from 'primeng/progressspinner';
import { InputText } from 'primeng/inputtext';
import { ValidationMessage } from '@/app/shared/validation-message';
import { NotificationService } from '@/app/shared/services/notification.service';
import { UsersService } from '@/app/proxy/users';
import { KeyFilterModule } from 'primeng/keyfilter';

@Component({
  selector: 'app-set-password',
  imports: [
    ReactiveFormsModule,
    Panel,
    BlockUI,
    ProgressSpinner,
    InputText,
    PanelModule,
    ValidationMessage,
    KeyFilterModule,
  ],
  template: `
    @if (form) {
      <form
        class="form-horizontal form-label-left"
        skipValidation
        [formGroup]="form"
        (ngSubmit)="saveChanges()"
      >
        <p-panel #pnlDetail [toggleable]="false" [collapsed]="false">
          <div class="formgrid grid">
            <div class="field col-12 md:col-12">
              <label for="newPassword">Mật khẩu <span class="required">*</span></label>
              <input
                id="newPassword"
                type="password"
                pInputPassword
                [pKeyFilter]="noSpecial"
                pInputText
                formControlName="newPassword"
                class="w-full"
              />
              <app-validation-message
                [entityForm]="form"
                fieldName="newPassword"
                [validationMessages]="validationMessages"
              >
              </app-validation-message>
            </div>
            <div class="field col-12 md:col-12">
              <label for="confirmPassword">Xác nhận mật khẩu <span class="required">*</span></label>
              <input
                id="confirmPassword"
                type="password"
                [pKeyFilter]="noSpecial"
                pInputText
                formControlName="confirmPassword"
                class="w-full"
              />
              <app-validation-message
                [entityForm]="form"
                fieldName="confirmPassword"
                [validationMessages]="validationMessages"
              >
              </app-validation-message>
            </div>
          </div>
          <ng-template pTemplate="footer">
            <button type="submit" class="btn btn-primary" [disabled]="!form.valid || btnDisabled">
              Lưu lại
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
export class SetPasword implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();

  public form!: FormGroup;
  @Input() userId: string = '';
  @Output() saveChange = new EventEmitter<void>();
  public saveBtnName!: string;
  public closeBtnName!: string;
  public blockedPanel: boolean = false;
  public btnDisabled = false;

  usersService = inject(UsersService);
  notificationService = inject(NotificationService);
  fb = inject(FormBuilder);
  // Validate
  noSpecial: RegExp = /^[^<>*!_~]+$/;
  validationMessages = {
    passsword: [
      { type: 'required', message: 'Bạn phải nhập mật khẩu' },
      {
        type: 'pattern',
        message: 'Mật khẩu ít nhất 8 ký tự, ít nhất 1 số, 1 ký tự đặc biệt, và một chữ hoa',
      },
    ],
    confirmPassword: [{ type: 'required', message: 'Xác nhận mật khẩu không đúng' }],
  };

  ngOnInit(): void {
    this.buildForm();
    this.saveBtnName = 'Cập nhật';
    this.closeBtnName = 'Đóng';
  }

  saveChanges() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.usersService
      .setPassword(this.userId, this.form.value)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: () => {
          this.toggleBlockUI(false);
          this.saveChange.emit();
          this.notificationService.showSuccess('Cập nhật mật khẩu thành công');
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  buildForm() {
    this.form = this.fb.group(
      {
        newPassword: [
          null,
          [
            Validators.required,
            Validators.pattern(
              /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{8,}$/,
            ),
          ],
        ],
        confirmPassword: [null],
      },
      { validators: passwordMatchingValidator }, // ✅ Sửa cấu hình validator ở form level
    );
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

// Hàm Validator (ở dưới hoặc file riêng):
export const passwordMatchingValidator: ValidatorFn = (
  control: AbstractControl,
): ValidationErrors | null => {
  const password = control.get('newPassword');
  const confirmPassword = control.get('confirmPassword'); // ✅ Đã sửa tên từ 'confirmNewPassword' thành 'confirmPassword'
  if (!password || !confirmPassword) {
    return null;
  }
  return password.value === confirmPassword.value ? null : { notmatched: true };
};
