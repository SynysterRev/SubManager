import { Component, output } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-add-subscription-modal',
  imports: [ReactiveFormsModule],
  templateUrl: './add-subscription-modal.html',
  styleUrl: './add-subscription-modal.scss'
})
export class AddSubscriptionModal {
  close = output<void>();
  add = output<any>();

  subForm: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required]),
    price: new FormControl('', [Validators.required]),
    paymentDay: new FormControl('', [Validators.required]),
    category: new FormControl('', [Validators.required])
  });

  closeModal(): void {
    this.close.emit();
  }

  addSubscription(): void {
    if (this.subForm.invalid) {
      this.subForm.markAllAsTouched();
      return;
    }

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
    this.add.emit("");
    this.closeModal();
  }
}
