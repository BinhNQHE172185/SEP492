import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CalendarModule } from 'primeng/calendar';
import { DialogModule } from 'primeng/dialog';
import { BudgetApiService } from '../../../../apis/budgetProposalAPIs/budget-api.service';
import { ButtonModule } from 'primeng/button';
import { ContractorApiService } from '../../../../apis/contractorAPIs/contractor-api.service';

@Component({
    selector: 'app-expert-detail',
    standalone: true,
    imports: [CalendarModule, FormsModule, DialogModule, CommonModule, ButtonModule],
    templateUrl: './expert-detail.component.html',
    styleUrl: './expert-detail.component.scss'
})
export class ExpertDetailComponent implements OnChanges {
    @Input() expertId!: string;
    @Input() displayDetailDialog: boolean = false;
    @Output() closeDialogEvent = new EventEmitter<void>();

    expert: any;

    constructor(private expertService: ContractorApiService) {}

    ngOnChanges(changes: SimpleChanges): void {
      console.log("true");
        if (changes['expertId'] && this.expertId) {
            this.loadProposalDetail();
        }
    }

    loadProposalDetail() {
        if (!this.expertId) return;

        this.expertService.getContractorDetail(this.expertId).subscribe(
            (response) => {
                this.expert = response;
                this.displayDetailDialog = true;
            },
            (error) => {
                console.error('Lỗi khi tải chi tiết báo cáo:', error);
            }
        );
    }

    closeDialog() {
        this.displayDetailDialog = false;
        this.closeDialogEvent.emit();
    }
}
