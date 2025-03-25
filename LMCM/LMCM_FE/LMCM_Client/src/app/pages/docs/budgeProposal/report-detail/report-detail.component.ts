import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CalendarModule } from 'primeng/calendar';
import { DialogModule } from 'primeng/dialog';
import { BudgetApiService } from '../../../../apis/budgetProposalAPIs/budget-api.service';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-report-detail',
  standalone: true,
  imports: [
    CalendarModule,
    FormsModule,
    DialogModule,
    CommonModule,
    ButtonModule
  ],
  templateUrl: './report-detail.component.html',
  styleUrl: './report-detail.component.scss'
})
export class ReportDetailComponent implements OnChanges {
  @Input() proposalId!: string;
  @Input() displayDetailDialog: boolean = false;
  @Output() closeDialogEvent = new EventEmitter<void>();

  report: any;
  file: any;

  constructor(private reportService: BudgetApiService) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['proposalId'] && this.proposalId) {
      this.loadProposalDetail();
    }
  }

  loadProposalDetail() {
    if (!this.proposalId) return;

    this.reportService.getBudgetDetail(this.proposalId).subscribe(
      (response) => {
        this.report = response;
        this.displayDetailDialog = true;
      },
      (error) => {
        console.error("Lỗi khi tải chi tiết báo cáo:", error);
      }
    );
  }

  closeDialog() {
    this.displayDetailDialog = false;
    this.closeDialogEvent.emit();
  }

  downloadFile(file: any) {
    if (file) {
      const link = document.createElement("a");
      link.href = file;
      link.download = this.file.split('/').pop();
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    }
  }
}

