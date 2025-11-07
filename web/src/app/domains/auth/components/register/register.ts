import { Component, inject, signal } from '@angular/core';
import { AuthLayout } from "../auth-layout/auth-layout";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { passwordMatchValidator } from '../validators/password-match.validator';
import { AuthService } from '../../services/auth';
import { CURRENCIES } from '../../../../shared/constants/currency';

@Component({
  selector: 'app-register',
  imports: [AuthLayout, ReactiveFormsModule, RouterModule],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class Register {

  authService = inject(AuthService);
  router = inject(Router);
  errorMessage = signal<string>('');

  registerForm: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(8)]),
    confirmPassword: new FormControl('', [Validators.required]),
    currency: new FormControl('EUR')
  }, { validators: passwordMatchValidator() });

  get email() { return this.registerForm.get('email')!; }
  get password() { return this.registerForm.get('password')!; }
  get confirmPassword() { return this.registerForm.get('confirmPassword')!; }


  onSubmit() {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }
    this.errorMessage.set('');

    this.authService.register(this.registerForm.value).subscribe({
      next: (tokenDto) => {
        console.log('Registered with email: ', tokenDto.email);
        this.router.navigate(['/dashboard']);
      },
      error: (err: string) => {
        this.errorMessage.set(err);
        console.error('Registration failed:', err);
      }
    });
  }

  getCurrencies() {
    return CURRENCIES;
  }
}
