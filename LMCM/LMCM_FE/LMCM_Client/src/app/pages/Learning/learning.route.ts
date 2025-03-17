import { Routes } from '@angular/router';
import { ListSubjectsComponent } from './list-subjects/list-subjects.component';
import { ListCurriculumComponent } from './list-curriculum/list-curriculum.component';
import { CurriculumDetailComponent } from './curriculum-detail/curriculum-detail.component';
import { PloComponent } from './plo/plo.component';
import { ListSyllabusComponent } from './list-syllabus/list-syllabus.component';
import { SyllabusDetailComponent } from './syllabus-detail/syllabus-detail.component';
import { HistoryOfChangeComponent } from './history-of-change/history-of-change.component';
import { ListReportComponent } from './list-report/list-report.component';
import { ListExpertComponent } from './list-expert/list-expert.component';

export default [
    { path: 'subject', component: ListSubjectsComponent },
    { path: 'curriculum', component: ListCurriculumComponent },
    { path: 'curriculum/:id', component: CurriculumDetailComponent },
    { path: 'curriculum/:id/plo', component: PloComponent },
    { path: 'history', component: HistoryOfChangeComponent },
    { path: 'listexpert', component: ListExpertComponent },
    { path: 'listreport', component: ListReportComponent },
    { path: 'syllabus', component: ListSyllabusComponent },
    { path: 'syllabus/:syllabusId', component: SyllabusDetailComponent },
    { path: 'edit-syllabus/:syllabusId', component: SyllabusDetailComponent }

] as Routes;