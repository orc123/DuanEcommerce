import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { AbstractControl, FormGroup } from '@angular/forms';

export interface ValidationMessageItem {
  type: string;
  message: string;
}

@Component({
  selector: 'app-validation-message',
  standalone: true,
  //changeDetection: ChangeDetectionStrategy.OnPush,
  styles: [
    `
      .p-error {
        color: #ef4444;
        font-size: 0.84rem;
        margin-top: 0.25rem;
        display: block;
        font-weight: 500;
      }
    `,
  ],
  template: `
    @if (control && control.invalid && (control.dirty || control.touched)) {
      <div>
        @for (validation of messages; track validation.type) {
          @if (control.hasError(validation.type)) {
            <small class="p-error">{{ validation.message }}.</small>
          }
        }
      </div>
    }
  `,
})
export class ValidationMessage {
  @Input() entityForm?: FormGroup;
  @Input() fieldName?: string;
  @Input() validationMessages?: Record<string, ValidationMessageItem[]> | any;

  get control(): AbstractControl | null {
    if (!this.entityForm || !this.fieldName) {
      return null;
    }
    return this.entityForm.get(this.fieldName);
  }

  get messages(): ValidationMessageItem[] {
    if (!this.validationMessages || !this.fieldName) {
      return [];
    }
    return this.validationMessages[this.fieldName] || [];
  }
}
