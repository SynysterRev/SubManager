import { Component, inject, signal } from '@angular/core';
import { AuthLayout } from "../auth-layout/auth-layout";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-login',
  imports: [AuthLayout, ReactiveFormsModule, RouterModule],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {
  authService = inject(AuthService);
  router = inject(Router);
  errorMessage = signal<string>('');

  get email() { return this.loginForm.get('email')!; }
  get password() { return this.loginForm.get('password')!; }

  loginForm: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl(''),
    confirmPassword: new FormControl('')
  });

  onSubmit() {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }
    this.errorMessage.set('');

    this.authService.login(this.loginForm.value).subscribe({
      next: (tokenDto) => {
        // console.log('Logged in with email: ', tokenDto.email);
        this.router.navigate(['/dashboard']);
      },
      error: (err: any) => {
        const message =
          err?.error && typeof err.error === 'string'
            ? err.error
            : err?.message || 'An error occured.';
        this.errorMessage.set(message);
        console.error('Login failed:', err);
      }
    });
  }
}
