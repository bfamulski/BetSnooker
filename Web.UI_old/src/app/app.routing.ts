import { Routes, RouterModule } from '@angular/router';

import { LoginComponent } from './login';
import { HomeComponent } from './home';
import { BetsComponent } from './bets';
import { RegisterComponent } from './register';
import { AuthGuard } from './_helpers';

const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'dashboard', component: HomeComponent, canActivate: [AuthGuard] },
    { path: 'bets', component: BetsComponent, canActivate: [AuthGuard] },
    { path: '', redirectTo: '/dashboard', pathMatch: 'full' }, // redirect to `dashboard`
    // otherwise redirect to home
    { path: '**', redirectTo: 'dashboard' }
];

export const appRoutingModule = RouterModule.forRoot(routes);