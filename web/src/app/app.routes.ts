import { Routes } from '@angular/router';
import { Register } from './domains/auth/components/register/register';
import { Login } from './domains/auth/components/login/login';

export const routes: Routes = [
    { path: 'register', component: Register },
    { path: 'login', component: Login },
    // { path: 'dashboard', component: }
];
