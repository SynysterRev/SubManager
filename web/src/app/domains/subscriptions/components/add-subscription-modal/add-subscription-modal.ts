import { Component, computed, effect, inject, input, output } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SubscriptionService } from '../../services/subscription';
import { SubscriptionDto, SubscriptionFormData } from '../../models/subscription.model';
import { ModalService } from '../../../../core/services/modal';
import { Category } from '../../models/category.model';
import { CURRENCIES } from '../../../../shared/constants/currency';
import { CurrencyPipe } from '../../pipes/currency-pipe';

@Component({
  selector: 'app-add-subscription-modal',
  imports: [ReactiveFormsModule, CurrencyPipe],
  templateUrl: './add-subscription-modal.html',
  styleUrl: './add-subscription-modal.scss'
})
export class AddSubscriptionModal {
  subService = inject(SubscriptionService);
  modalService = inject(ModalService);

  get price() { return this.subForm.get('price')!; }
  get paymentDay() { return this.subForm.get('paymentDay')!; }

  subscription = input<SubscriptionDto | undefined>(undefined);
  categories = input<Category[]>();
  currencyCode = input.required<string>();
  submitForm = output<SubscriptionFormData>();

  isEdit = computed(() => !!this.subscription());

  userLocale = navigator.language;

  subForm: FormGroup;

  constructor() {
    this.subForm = new FormGroup({
      name: new FormControl(this.subscription()?.name ?? '', [Validators.required, Validators.minLength(3)]),
      price: new FormControl(this.subscription()?.price ?? '', [Validators.required, Validators.min(0)]),
      paymentDay: new FormControl(this.subscription()?.paymentDay ?? '', [Validators.required, Validators.min(1),
      Validators.max(31)]),
      categoryId: new FormControl(this.subscription()?.categoryId ?? '',),
    });

    effect(() => {
      const sub = this.subscription();
      if (sub) {
        this.subForm.patchValue({
          name: sub.name,
          price: sub.price,
          paymentDay: sub.paymentDay,
          categoryId: sub.categoryId
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

  getCurrencies() {
    return CURRENCIES;
  }
}
