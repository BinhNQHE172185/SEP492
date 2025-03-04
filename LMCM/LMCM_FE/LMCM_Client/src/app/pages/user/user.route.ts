import { Routes } from '@angular/router';
import { Login } from '../auth/Login/login';
import { StaffManageComponent } from './staff-manage/staff-manage.component';
import { ProfileComponent } from './profile/profile.component';

export default [
    { path: 'staff', component: StaffManageComponent },
    { path: 'profile', component: ProfileComponent },
] as Routes;
