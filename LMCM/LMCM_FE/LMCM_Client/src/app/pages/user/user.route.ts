import { Routes } from '@angular/router';
import { Login } from '../auth/Login/login';
import { StaffManageComponent } from './staff-manage/staff-manage.component';
import { ProfileComponent } from './profile/profile.component';
import { ListSubjectsComponent } from './list-subjects/list-subjects.component';
import { ListCurriculumnComponent } from './list-curriculumn/list-curriculumn.component';
import { ListSyllabusComponent } from './list-syllabus/list-syllabus.component';
import { PloComponent } from './plo/plo.component';
import { CurriculumDetailComponent } from './curriculum-detail/curriculum-detail.component';
import { SyllabusDetailComponent } from './syllabus-detail/syllabus-detail.component';


export default [
    { path: 'staff', component: StaffManageComponent },
    { path: 'profile', component: ProfileComponent },
    { path: 'listsubject', component: ListSubjectsComponent },
    {path:'listcurriculumn',component:ListCurriculumnComponent},
    {path:'curriculumn',component:CurriculumDetailComponent},
    {path:'plo',component:PloComponent},

    {path:'listsyllabus',component:ListSyllabusComponent},
    {path:'syllabus',component:SyllabusDetailComponent},
    {path: 'syllabus/:syllabusId', component: SyllabusDetailComponent }, // ✅ Dynamic route for syllabus details
    { path: 'edit-syllabus/:syllabusId', component: SyllabusDetailComponent } // ✅ Dynamic route for editing syllabus
  
] as Routes;
