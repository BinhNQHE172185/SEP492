import { Routes } from '@angular/router';
import { ListReportComponent } from '../docs/list-report/list-report.component';
import { ListExpertComponent } from '../docs/list-expert/list-expert.component';

export default [
    { path: 'expert', component: ListExpertComponent },
    { path: 'report', component: ListReportComponent },
] as Routes;