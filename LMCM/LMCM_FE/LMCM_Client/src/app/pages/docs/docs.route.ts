import { Routes } from '@angular/router';
import { ListReportComponent } from '../docs/budgeProposal/list-report/list-report.component';
import { ListExpertComponent } from './contractor/list-expert/list-expert.component';
import { ListAcceptanceReportComponent } from './acceptanceReport/list-acceptance-report/list-acceptance-report.component';
import { ListContractComponent } from './contract/list-contract/list-contract.component';
import { ListTemplateComponent } from './template/list-template/list-template.component';

export default [
    { path: 'expert', component: ListExpertComponent },
    { path: 'expert/:id', component: ListExpertComponent },
    { path: 'report', component: ListReportComponent },
    { path: 'report/:id', component: ListReportComponent },
    { path: 'acceptance-report', component: ListAcceptanceReportComponent },
    { path: 'contract', component: ListContractComponent },
    { path: 'contract/:id', component: ListContractComponent },
    { path: 'template', component: ListTemplateComponent },
] as Routes;
