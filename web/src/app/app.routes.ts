import { Routes } from '@angular/router';
import { Register } from './domains/auth/components/auth/register/register';
import { Login } from './domains/auth/components/auth/login/login';

export const routes: Routes = [
    { path: 'register', component: Register },
    { path: 'login', component: Login },
];
