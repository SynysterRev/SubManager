import { Routes } from '@angular/router';
import { Register } from './domains/auth/components/register/register';
import { Login } from './domains/auth/components/login/login';
import { SubscriptionsDashboard } from './domains/subscriptions/components/subscriptions-dashboard/subscriptions-dashboard';
import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
    { path: 'register', component: Register },
    { path: 'login', component: Login },
    {
        path: 'dashboard', component: SubscriptionsDashboard,
        canActivate: [authGuard]
    },
];
