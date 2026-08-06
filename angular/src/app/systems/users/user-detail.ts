import { Component, EventEmitter, inject, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { forkJoin, Subject, takeUntil } from 'rxjs';
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
import { ValidationMessage } from '@/app/shared/validation-message';
import { RolesService } from '@/app/proxy/roles/roles.service';
import { UtilityService } from '@/app/shared/services/utility.service';
import { NotificationService } from '@/app/shared/services/notification.service';
import { MessageConstants } from '@/app/shared/constants/message.const';
import { UserDto, UsersService } from '@/app/proxy/users';
import { RoleDto } from '@/app/proxy/roles';
@Component({
  selector: 'app-user-detail',
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
              <label for="userName">Tài khoản <span class="required">*</span></label>
              <input
                id="userName"
                type="text"
                pInputText
                formControlName="userName"
                class="w-full"
              />
              <app-validation-message
                [entityForm]="form"
                fieldName="userName"
                [validationMessages]="validationMessages"
              >
              </app-validation-message>
            </div>
            <div class="field col-12 md:col-6">
              <label for="password">Mật khẩu <span class="required">*</span></label>
              <input
                id="password"
                type="password"
                pInputText
                formControlName="password"
                class="w-full"
              />
              <app-validation-message
                [entityForm]="form"
                fieldName="password"
                [validationMessages]="validationMessages"
              >
              </app-validation-message>
            </div>
            <div class="field col-12 md:col-6">
              <label for="name" class="block">Tên <span class="required">*</span></label>
              <input id="name" type="text" pInputText formControlName="name" class="w-full" />
              <app-validation-message
                [entityForm]="form"
                fieldName="name"
                [validationMessages]="validationMessages"
              >
              </app-validation-message>
            </div>
            <div class="field col-12 md:col-6">
              <label for="phoneNumber">Số điện thoại<span class="required">*</span></label>
              <input
                id="phoneNumber"
                type="text"
                pInputText
                formControlName="phoneNumber"
                class="w-full"
              />
              <app-validation-message
                [entityForm]="form"
                fieldName="phoneNumber"
                [validationMessages]="validationMessages"
              >
              </app-validation-message>
            </div>
            <div class="field col-12 md:col-6">
              <label for="surname">Họ<span class="required">*</span></label>
              <input id="surname" type="text" pInputText formControlName="surname" class="w-full" />
              <app-validation-message
                [entityForm]="form"
                fieldName="surname"
                [validationMessages]="validationMessages"
              >
              </app-validation-message>
            </div>
            <div class="field col-12 md:col-6">
              <label for="email">Email <span class="required">*</span></label>
              <input id="email" type="text" pInputText formControlName="email" class="w-full" />
              <app-validation-message
                [entityForm]="form"
                fieldName="email"
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
export class UserDetail implements OnInit, OnDestroy {
  fb = inject(FormBuilder);
  userService = inject(UsersService);
  utilityService = inject(UtilityService);
  notificationService = inject(NotificationService);
  roleService = inject(RolesService);

  private ngUnsubscribe = new Subject<void>();
  public form!: FormGroup;

  selectedEntity = {} as UserDto;

  dataTypes: any[] = [];
  public thumbnailImage: any;
  @Input() userId: string = '';
  @Output() saveChange = new EventEmitter<void>();
  public saveBtnName!: string;
  public closeBtnName!: string;
  public blockedPanel: boolean = false;
  public roles: any[] = [];
  public btnDisabled = false;

  // Validate
  validationMessages = {
    name: [{ type: 'required', message: 'Bạn phải nhập tên' }],
    surname: [{ type: 'required', message: 'Bạn phải URL duy nhất' }],
    email: [{ type: 'required', message: 'Bạn phải nhập email' }],
    userName: [{ type: 'required', message: 'Bạn phải nhập tài khoản' }],
    password: [
      { type: 'required', message: 'Bạn phải nhập mật khẩu' },
      {
        type: 'pattern',
        message: 'Mật khẩu ít nhất 8 ký tự, ít nhất 1 số, 1 ký tự đặc biệt, và một chữ hoa',
      },
    ],
    phoneNumber: [{ type: 'required', message: 'Bạn phải nhập số điện thoại' }],
  };

  ngOnInit() {
    //Init form
    this.buildForm();
    //Load data to form
    var roles = this.roleService.getListAll();
    this.toggleBlockUI(true);
    forkJoin({
      roles,
    })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (repsonse: any) => {
          //Push categories to dropdown list
          var roles = repsonse.roles as RoleDto[];
          roles.forEach(element => {
            this.roles.push({
              value: element.id,
              label: element.name,
            });
          });

          if (this.utilityService.isEmpty(this.userId) == false) {
            this.loadFormDetails(this.userId);
          } else {
            this.setMode('create');
            this.toggleBlockUI(false);
          }
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }
  loadFormDetails(id: string) {
    this.userService
      .get(id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: UserDto) => {
          this.selectedEntity = response;
          this.buildForm();
          this.setMode('update');

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
    var message = this.utilityService.isEmpty(this.userId)
      ? MessageConstants.CREATED_OK_MSG
      : MessageConstants.UPDATED_OK_MSG;
    this.toggleBlockUI(true);
    const saveObservable = this.utilityService.isEmpty(this.userId)
      ? this.userService.create(this.form.value)
      : this.userService.update(this.userId, this.form.value);

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
  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.btnDisabled = true;
      this.blockedPanel = true;
    } else {
      setTimeout(() => {
        this.btnDisabled = false;
        this.blockedPanel = false;
      }, 1000);
    }
  }

  setMode(mode: string) {
    if (mode == 'update') {
      this.form.controls['userName'].clearValidators();
      this.form.controls['userName'].disable();
      this.form.controls['email'].clearValidators();
      this.form.controls['email'].disable();
      this.form.controls['password'].clearValidators();
      this.form.controls['password'].disable();
    } else if (mode == 'create') {
      this.form.controls['userName'].addValidators(Validators.required);
      this.form.controls['userName'].enable();
      this.form.controls['email'].addValidators(Validators.required);
      this.form.controls['email'].enable();
      this.form.controls['password'].addValidators(Validators.required);
      this.form.controls['password'].enable();
    }
  }
  buildForm() {
    this.form = this.fb.group({
      name: new FormControl(this.selectedEntity.name || null, Validators.required),
      surname: new FormControl(this.selectedEntity.surname || null, Validators.required),
      userName: new FormControl(this.selectedEntity.userName || null, Validators.required),
      email: new FormControl(this.selectedEntity.email || null, Validators.required),
      phoneNumber: new FormControl(this.selectedEntity.phoneNumber || null, Validators.required),
      password: new FormControl(
        null,
        Validators.compose([
          Validators.required,
          Validators.pattern(
            '^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-zd$@$!%*?&].{8,}$',
          ),
        ]),
      ),
    });
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
}
