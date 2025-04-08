import { Component } from '@angular/core';
import { StatsWidget } from './components/statswidget';
import { ReportOfBuilding } from './components/reportOfBuilding';
import { timeOfBuilding } from './components/timeOfBuilding';

@Component({
    selector: 'app-dashboard',
    imports: [StatsWidget, ReportOfBuilding, timeOfBuilding],
    template: `
        <div class="grid grid-cols-12 gap-8">
            <app-stats-widget class="contents" />
            <div class="col-span-12 xl:col-span-6">
                <app-time-of-building />
            </div>
            <div class="col-span-12 xl:col-span-6">
                <app-report-of-builing />
            </div>
        </div>
    `
})
export class Dashboard { }
