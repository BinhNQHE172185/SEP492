import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardApiService } from '../../../apis/dashboardAPIs/dashboard-api';

@Component({
    standalone: true,
    selector: 'app-stats-widget',
    imports: [CommonModule],
    template: `
        <div class="col-span-12 lg:col-span-6 xl:col-span-3">
            <div class="card mb-0">
                <div class="flex justify-between mb-4">
                    <div>
                        <span class="block text-muted-color font-medium mb-4">Môn học</span>
                        <div class="text-surface-900 dark:text-surface-0 font-medium text-xl">{{ items?.subjectCount || 0 }}</div>
                    </div>
                    <div class="flex items-center justify-center bg-blue-100 dark:bg-blue-400/10 rounded-border" style="width: 2.5rem; height: 2.5rem">
                        <i class="pi pi-book text-blue-500 !text-xl"></i>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-span-12 lg:col-span-6 xl:col-span-3">
            <div class="card mb-0">
                <div class="flex justify-between mb-4">
                    <div>
                        <span class="block text-muted-color font-medium mb-4">Đề cương</span>
                        <div class="text-surface-900 dark:text-surface-0 font-medium text-xl">{{ items?.syllabusCount || 0 }}</div>
                    </div>
                    <div class="flex items-center justify-center bg-blue-100 dark:bg-blue-400/10 rounded-border" style="width: 2.5rem; height: 2.5rem">
                        <i class="pi pi-book text-blue-500 !text-xl"></i>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-span-12 lg:col-span-6 xl:col-span-3">
            <div class="card mb-0">
                <div class="flex justify-between mb-4">
                    <div>
                        <span class="block text-muted-color font-medium mb-4">Chương trình</span>
                        <div class="text-surface-900 dark:text-surface-0 font-medium text-xl"> {{ items?.curriculumnCount || 0 }}</div>
                    </div>
                    <div class="flex items-center justify-center bg-blue-100 dark:bg-blue-400/10 rounded-border" style="width: 2.5rem; height: 2.5rem">
                        <i class="pi pi-book text-blue-500 !text-xl"></i>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-span-12 lg:col-span-6 xl:col-span-3">
            <div class="card mb-0">
                <div class="flex justify-between mb-4">
                    <div>
                        <span class="block text-muted-color font-medium mb-4">Nhân sự</span>
                        <div class="text-surface-900 dark:text-surface-0 font-medium text-xl">{{ items?.userCount || 0 }}</div>
                    </div>
                    <div class="flex items-center justify-center bg-cyan-100 dark:bg-cyan-400/10 rounded-border" style="width: 2.5rem; height: 2.5rem">
                        <i class="pi pi-users text-cyan-500 !text-xl"></i>
                    </div>
                </div>
            </div>
        </div>
        `
})
export class StatsWidget implements OnInit {
    items: any;
    constructor(
        private dashboardApiService: DashboardApiService,
    ) {
    }

    ngOnInit() {
        this.dashboardApiService.getItemData().subscribe(
            (response) => {
                this.items = response;
            },
            (error) => {
                console.error('Error fetching item data', error);
            }
        );
    }
}
