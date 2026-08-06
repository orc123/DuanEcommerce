import {
  Component,
  EventEmitter,
  inject,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  Output,
  SimpleChanges,
} from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { PanelModule } from 'primeng/panel';
import { BlockUIModule } from 'primeng/blockui';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ButtonModule } from 'primeng/button';
import { NotificationService } from '../shared/services/notification.service';
import { RolesService } from '../proxy/roles';
import {
  GetPermissionListResultDto,
  PermissionGrantInfoDto,
  PermissionGroupDto,
  UpdatePermissionDto,
  UpdatePermissionsDto,
} from '../proxy/volo/abp/permission-management';
import { CheckboxModule } from 'primeng/checkbox';
import { FormsModule } from '@angular/forms';
@Component({
  selector: 'app-permission-grant',
  imports: [
    ReactiveFormsModule,
    FormsModule,
    BlockUIModule,
    ProgressSpinnerModule,
    PanelModule,
    CheckboxModule,
    ButtonModule,
  ],
  template: `
    @if (form) {
      <form
        class="form-horizontal form-label-left"
        skipValidation
        [formGroup]="form"
        (ngSubmit)="saveChanges()"
      >
        <p-panel #pnlDetail header="Phân quyền cho role" [toggleable]="true" [collapsed]="false">
          @for (group of groups; track group.name) {
            <div class="mb-4">
              <h3 class="font-bold text-lg mb-2">{{ group.displayName }}</h3>
              @for (permission of group.permissions; track permission.name) {
                <div class="field-checkbox my-2">
                  <p-checkbox
                    [value]="permission.name"
                    [(ngModel)]="selectedPermissions"
                    [inputId]="permission.name"
                    [ngModelOptions]="{ standalone: true }"
                  ></p-checkbox>
                  <label [for]="permission.name" class="ml-2 cursor-pointer">{{
                    permission.displayName
                  }}</label>
                </div>
              }
            </div>
          }
          <ng-template pTemplate="footer">
            <button
              type="submit"
              pButton
              class="p-button-primary"
              [disabled]="!form.valid || btnDisabled"
            >
              {{ saveBtnName }}
            </button>
          </ng-template>
          <p-blockUI [target]="pnlDetail" [blocked]="blockedPanelDetail">
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
export class PermissionGrant implements OnInit, OnChanges, OnDestroy {
  fb = inject(FormBuilder);
  rolesService = inject(RolesService);
  notificationService = inject(NotificationService);
  private ngUnsubscribe = new Subject<void>();
  public blockedPanelDetail: boolean = false;
  public form!: FormGroup;
  public btnDisabled = false;
  public saveBtnName!: string;
  public closeBtnName!: string;
  public groups: PermissionGroupDto[] = [];
  public permissions: PermissionGrantInfoDto[] = [];
  public selectedPermissions: string[] = [];
  @Input() providerKey: string = '';
  @Output() saveChange = new EventEmitter<void>();
  ngOnInit() {
    this.buildForm();
    this.saveBtnName = 'Cập nhật';
    this.closeBtnName = 'Hủy';
  }
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['providerKey'] && changes['providerKey'].currentValue) {
      this.loadDetail(this.providerKey, 'R');
    }
  }
  loadDetail(providerKey: string, providerName: string) {
    this.toggleBlockUI(true);
    this.groups = [];
    this.permissions = [];
    this.selectedPermissions = [];
    this.rolesService
      .getPermissions(providerName, providerKey)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: GetPermissionListResultDto) => {
          this.groups = response?.groups || [];
          this.groups.forEach((group: PermissionGroupDto) => {
            group.permissions?.forEach((permission: PermissionGrantInfoDto) => {
              if (permission && permission.name) {
                this.permissions.push(permission);
                if (permission.isGranted) {
                  this.selectedPermissions.push(permission.name);
                }
              }
            });
          });
          this.buildForm();
          this.toggleBlockUI(false);
        },
        error: err => {
          this.notificationService.showError(
            err?.error?.error?.message || 'Không thể tải danh sách quyền',
          );
          this.toggleBlockUI(false);
        },
      });
  }
  saveChanges() {
    this.toggleBlockUI(true);
    this.saveData();
  }
  private saveData() {
    var permissions: UpdatePermissionDto[] = [];
    for (let index = 0; index < this.permissions.length; index++) {
      const isGranted = this.selectedPermissions.includes(this.permissions[index].name!);
      permissions.push({ name: this.permissions[index].name, isGranted: isGranted });
    }
    var updateValues: UpdatePermissionsDto = {
      permissions: permissions,
    };
    this.rolesService
      .updatePermissions('R', this.providerKey, updateValues)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: () => {
          this.notificationService.showSuccess('Cập nhật quyền thành công');
          this.toggleBlockUI(false);
          this.saveChange.emit();
        },
        error: err => {
          this.notificationService.showError(err?.error?.error?.message || 'Có lỗi xảy ra');
          this.toggleBlockUI(false);
        },
      });
  }
  buildForm() {
    this.form = this.fb.group({});
  }
  private toggleBlockUI(enabled: boolean) {
    if (enabled) {
      this.btnDisabled = true;
      this.blockedPanelDetail = true;
    } else {
      setTimeout(() => {
        this.btnDisabled = false;
        this.blockedPanelDetail = false;
      }, 300);
    }
  }
  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
}
