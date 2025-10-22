import { Component, inject, output } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SubscriptionService } from '../../services/subscription';
import { SubscriptionDto } from '../../models/subscription.model';
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


  subForm: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required]),
    price: new FormControl('', [Validators.required]),
    paymentDay: new FormControl('', [Validators.required, Validators.min(1),
    Validators.max(31)]),
    category: new FormControl('',)
  });

  closeModal(): void {
    this.modalService.closeModal();
  }

  addSubscription(): void {
    if (this.subForm.invalid) {
      this.subForm.markAllAsTouched();
      return;
    }

    this.subService.createNewSubcription(this.subForm.value).subscribe({
      next: (newSub) => {
        this.modalService.notifyModalData('addSubscription', newSub);
        this.closeModal();
      }
    });

  }
}
