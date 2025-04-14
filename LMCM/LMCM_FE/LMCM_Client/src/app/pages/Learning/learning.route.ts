import { Routes } from '@angular/router';
import { ListSubjectsComponent } from './list-subjects/list-subjects.component';
import { ListCurriculumComponent } from './list-curriculum/list-curriculum.component';
import { CurriculumDetailComponent } from './curriculum-detail/curriculum-detail.component';
import { PloComponent } from './plo/plo.component';
import { ListSyllabusComponent } from './list-syllabus/list-syllabus.component';
import { SyllabusDetailComponent } from './syllabus-detail/syllabus-detail.component';
import { HistoryOfChangeComponent } from './history-of-change/history-of-change.component';
import { SyllabusHistoryComponent } from './syllabus-history/syllabus-history.component';
import { SubjectHistoryComponent } from './subject-history/subject-history.component';

export default [
    { path: 'subject', component: ListSubjectsComponent },
    { path: 'curriculum', component: ListCurriculumComponent },
    { path: 'curriculum/:id', component: CurriculumDetailComponent },
    { path: 'curriculum/:id/plo', component: PloComponent },
    { path: 'history', component: HistoryOfChangeComponent },
    { path: 'syllabus', component: ListSyllabusComponent },
    { path: 'syllabus/:id', component: SyllabusDetailComponent },
    { path: 'syllabus-history/:id', component: SyllabusHistoryComponent },
    { path: 'subject-history/:id', component: SubjectHistoryComponent },
] as Routes;