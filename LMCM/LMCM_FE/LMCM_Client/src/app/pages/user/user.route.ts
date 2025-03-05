import { Routes } from '@angular/router';
import { Login } from '../auth/Login/login';
import { StaffManageComponent } from './staff-manage/staff-manage.component';
import { ProfileComponent } from './profile/profile.component';
import { ListSubjectsComponent } from './list-subjects/list-subjects.component';
import { ListCurriculumnComponent } from './list-curriculumn/list-curriculumn.component';
import { ListSyllabusComponent } from './list-syllabus/list-syllabus.component';


export default [
    { path: 'staff', component: StaffManageComponent },
    { path: 'profile', component: ProfileComponent },
    { path: 'listsubject', component: ListSubjectsComponent },
    {path:'listcurriculumn',component:ListCurriculumnComponent},
    {path:'listsyllabus',component:ListSyllabusComponent}
] as Routes;
