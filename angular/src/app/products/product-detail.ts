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
  ],
  template: `
    <form [formGroup]="form" (ngSubmit)="saveChanges()">
      <p-panel #pnl header="Sản phẩm">
        <div class="form-grid grid">
          <label for="name" class="block">Tên</label>
          <input id="name" pInputText type="text" formControlName="name" />
        </div>
        <ng-template pTemplate="footer">
          <button type="submit" pButton icon="fa fa-save" label="lưu lại"></button>
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
      console.log(response);
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
