import { Component, effect, inject, input, output } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SubscriptionService } from '../../services/subscription';
import { SubscriptionDto, SubscriptionFormData } from '../../models/subscription.model';
import { DecimalPipe } from '@angular/common';
import { ModalService } from '../../../../core/services/modal';

@Component({
  selector: 'app-add-subscription-modal',
  imports: [ReactiveFormsModule, DecimalPipe],
  templateUrl: './add-subscription-modal.html',
  styleUrl: './add-subscription-modal.scss'
})
export class AddSubscriptionModal {
  subService = inject(SubscriptionService);
  modalService = inject(ModalService);

  get price() { return this.subForm.get('price')!; }
  get paymentDay() { return this.subForm.get('paymentDay')!; }

  subscription = input<SubscriptionDto>();
  submitForm = output<SubscriptionFormData>();

  isEdit: boolean = this.subscription() !== null;

  subForm: FormGroup;

  constructor() {
    this.subForm = new FormGroup({
      name: new FormControl(this.subscription()?.name ?? '', [Validators.required]),
      price: new FormControl(this.subscription()?.price ?? '', [Validators.required]),
      paymentDay: new FormControl(this.subscription()?.paymentDay ?? '', [Validators.required, Validators.min(1),
      Validators.max(31)]),
      category: new FormControl(this.subscription()?.category ?? '',)
    });

    effect(() => {
      const sub = this.subscription();
      if (sub) {
        this.subForm.patchValue({
          name: sub.name,
          price: sub.price,
          paymentDay: sub.paymentDay,
          category: sub.category
        });
      }
    });
  }

  closeModal(): void {
    this.modalService.closeModal();
  }

  onSubmit(): void {
    if (this.subForm.invalid) {
      this.subForm.markAllAsTouched();
      return;
    }
    this.submitForm.emit(this.subForm.value);
  }
}
