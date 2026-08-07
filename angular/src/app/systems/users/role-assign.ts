import { RolesService } from '@/app/proxy/roles';
import { UsersService } from '@/app/proxy/users';
import { Component, EventEmitter, inject, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { PanelModule } from 'primeng/panel';
import { BlockUI } from 'primeng/blockui';
import { ProgressSpinner } from 'primeng/progressspinner';
import { PickListModule } from 'primeng/picklist';
import { NotificationService } from '@/app/shared/services/notification.service';

@Component({
  selector: 'app-role-assign',
  imports: [PanelModule, BlockUI, ProgressSpinner, PickListModule],
  template: `
    <p-panel #pnlDetail header="Thông tin" [toggleable]="true" [collapsed]="false">
      <p-pickList
        [source]="avaiableRoles"
        [target]="selectedRoles"
        sourceHeader="Các quyền có sẵn"
        targetHeader="Các quyền chọn"
        [dragdrop]="true"
      >
        <ng-template let-role pTemplate="item">
          <div>
            {{ role }}
          </div>
        </ng-template>
      </p-pickList>
      <ng-template pTemplate="footer">
        <button type="button" (click)="saveChanges()" class="btn btn-primary">Lưu lại</button>
      </ng-template>

      <p-block-ui [target]="pnlDetail" [blocked]="blockedPanel">
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
      </p-block-ui>
    </p-panel>
  `,
})
export class RoleAssign implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  public avaiableRoles: string[] = [];
  public selectedRoles: string[] = [];
  public blockedPanel: boolean = false;
  public form!: FormGroup;

  @Input() userId: string = '';
  @Output() saveChange = new EventEmitter<void>();

  userService = inject(UsersService);
  roleService = inject(RolesService);
  notificationService = inject(NotificationService);
  ngOnInit(): void {
    this.toggleBlockUI(true);
    var roles = this.roleService.getListAll();
    forkJoin({
      roles,
    })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: res => {
          var roles = res.roles;
          roles.forEach(e => {
            this.avaiableRoles.push(e.name!);
            this.loadDetail(this.userId);
          });
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  loadDetail(id: string) {
    this.toggleBlockUI(true);
    this.userService
      .get(id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: res => {
          this.selectedRoles = res.roles!;
          this.avaiableRoles = this.avaiableRoles.filter(x => !this.selectedRoles.includes(x));
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  saveChanges() {
    this.userService
      .assignRole(this.userId, this.selectedRoles)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: () => {
          this.toggleBlockUI(false);
          this.notificationService.showSuccess('Gán quyền thành công');
          this.saveChange.emit();
        },
      });
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
