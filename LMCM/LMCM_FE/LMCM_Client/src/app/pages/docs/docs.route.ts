import { Routes } from '@angular/router';
import { ListReportComponent } from '../docs/list-report/list-report.component';
import { ListExpertComponent } from '../docs/list-expert/list-expert.component';
import { ListAcceptanceReportComponent } from './list-acceptance-report/list-acceptance-report.component';

export default [
    { path: 'expert', component: ListExpertComponent },
    { path: 'report', component: ListReportComponent },
    { path: 'acceptance-report', component: ListAcceptanceReportComponent },

] as Routes;