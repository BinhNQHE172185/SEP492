import { Routes } from '@angular/router';
import { ListSubjectsComponent } from './list-subjects/list-subjects.component';
import { ListCurriculumComponent } from './list-curriculum/list-curriculum.component';
import { CurriculumDetailComponent } from './curriculum-detail/curriculum-detail.component';
import { PloComponent } from './plo/plo.component';
import { ListSyllabusComponent } from './list-syllabus/list-syllabus.component';
import { SyllabusDetailComponent } from './syllabus-detail/syllabus-detail.component';
import { HistoryOfChangeComponent } from './history-of-change/history-of-change.component';

export default [
    { path: 'listsubject', component: ListSubjectsComponent },
    {path:'listcurriculum',component:ListCurriculumComponent},
    {path:'curriculum',component:CurriculumDetailComponent},
    {path:'curriculum/:code',component:CurriculumDetailComponent},
    {path:'plo',component:PloComponent},
    {path:'history',component:HistoryOfChangeComponent},
    { path: 'listsyllabus', component: ListSyllabusComponent },
    { path: 'syllabus', component: SyllabusDetailComponent },
    { path: 'syllabus/:syllabusId', component: SyllabusDetailComponent },
    { path: 'edit-syllabus/:syllabusId', component: SyllabusDetailComponent } 

] as Routes;