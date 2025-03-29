import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CalendarModule } from 'primeng/calendar';
import { DialogModule } from 'primeng/dialog';
import { BudgetApiService } from '../../../../apis/budgetProposalAPIs/budget-api.service';
import { ButtonModule } from 'primeng/button';
import { ContractorApiService } from '../../../../apis/contractorAPIs/contractor-api.service';
import { DocumentTemplateApiService } from '../../../../apis/templateAPIs/template-api.service';

@Component({
    selector: 'app-template-detail',
    standalone: true,
    imports: [CalendarModule, FormsModule, DialogModule, CommonModule, ButtonModule],
    templateUrl: './template-detail.component.html',
    styleUrl: './template-detail.component.scss'
})
export class TemplateDetailComponent implements OnChanges {
    @Input() templateId!: string;
    @Input() displayDetailDialog: boolean = false;
    @Output() closeDialogEvent = new EventEmitter<void>();

    template: any;

    constructor(private templateService: DocumentTemplateApiService) {}

    ngOnChanges(changes: SimpleChanges): void {
      console.log("templateId received:", this.templateId);
      console.log("true");
        if (changes['templateId'] && this.templateId) {
            this.loadTemplateDetail();
        }
    }

    loadTemplateDetail() {
        if (!this.templateId) return;

        this.templateService.getTemplateDetail(this.templateId).subscribe(
            (response) => {
                this.template = response;
                this.displayDetailDialog = true;
            },
            (error) => {
                console.error('Lỗi khi tải chi tiết báo cáo:', error);
            }
        );
    }
    downloadFile(url: string) {
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', 'file');
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    }
    
    closeDialog() {
        this.displayDetailDialog = false;
        this.closeDialogEvent.emit();
    }
}
