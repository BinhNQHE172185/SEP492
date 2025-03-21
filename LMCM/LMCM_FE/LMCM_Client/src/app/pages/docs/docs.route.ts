import { Routes } from '@angular/router';
import { ListReportComponent } from '../docs/list-report/list-report.component';
import { ListExpertComponent } from '../docs/list-expert/list-expert.component';
import { ListContractComponent } from './list-contract/list-contract.component';

export default [
    { path: 'expert', component: ListExpertComponent },
    { path: 'report', component: ListReportComponent },
    { path: 'contract', component: ListContractComponent },
] as Routes;