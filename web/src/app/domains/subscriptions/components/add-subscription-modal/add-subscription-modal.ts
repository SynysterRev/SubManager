import { Component, inject, output } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SubscriptionService } from '../../services/subscription';
import { SubscriptionDto } from '../../models/subscription.model';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-add-subscription-modal',
  imports: [ReactiveFormsModule, DecimalPipe],
  templateUrl: './add-subscription-modal.html',
  styleUrl: './add-subscription-modal.scss'
})
export class AddSubscriptionModal {
  close = output<void>();
  add = output<SubscriptionDto>();
  subService = inject(SubscriptionService);

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
    this.close.emit();
  }

  addSubscription(): void {
    if (this.subForm.invalid) {
      this.subForm.markAllAsTouched();
      return;
    }

    this.subService.createNewSubcription(this.subForm.value).subscribe({
      next: (newSub) => {
        this.add.emit(newSub);
        this.closeModal();
      }
    });

    // this.authService.login(this.loginForm.value).subscribe({
    //   next: (tokenDto) => {
    //     console.log('Logged in with email: ', tokenDto.email);
    //     this.router.navigate(['/dashboard']);
    //   },
    //   error: (err: any) => {
    //     const message =
    //       err?.error && typeof err.error === 'string'
    //         ? err.error
    //         : err?.message || 'An error occured.';
    //     this.errorMessage.set(message);
    //     console.error('Login failed:', err);
    //   }
    // });
    // Logique de validation et de collecte des données du formulaire ici...

    // Une fois la logique terminée, émettre l'événement

  }
}
