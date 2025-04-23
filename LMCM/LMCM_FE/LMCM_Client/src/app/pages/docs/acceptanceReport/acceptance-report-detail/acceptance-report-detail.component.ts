import { Component, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CalendarModule } from 'primeng/calendar';
import { ContractApiService } from '../../../../apis/contractAPIs/contract-api.service';
import { ToastModule } from 'primeng/toast';
import { AcceptanceRecordApiService } from '../../../../apis/acceptanceRecordAPIs/acceptance-api.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-acceptance-report-detail',
  imports: [CalendarModule, FormsModule, DialogModule, CommonModule, ButtonModule, ToastModule],
  templateUrl: './acceptance-report-detail.component.html',
  styleUrl: './acceptance-report-detail.component.scss',
  providers: [ConfirmationService, MessageService]
})
export class AcceptanceReportDetailComponent {
  @Input() acceptanceId!: string;
  @Input() displayDetailDialog: boolean = false;
  @Output() closeDialogEvent = new EventEmitter<void>();

  uploadedFiles: any[] = [];
  file: any;
  calendarValue: any = null;
  contract: any;

  constructor(
    private messageService: MessageService,
    private acceptanceService: AcceptanceRecordApiService,
    private router: Router
  ) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['acceptanceId'] && this.acceptanceId) {
      this.loadContractDetail();
    }
  }

  loadContractDetail() {
    if (!this.acceptanceId) return;

    this.acceptanceService.getAcceptanceRecordDetail(this.acceptanceId).subscribe(
      (response) => {
        this.contract = response;
        this.displayDetailDialog = true;
      },
      (error) => {
        this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: error.error.message });
      }
    );
  }

  closeDialog() {
    this.displayDetailDialog = false;
    this.closeDialogEvent.emit();
  }

  downloadFile(url: string) {
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', 'file');
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }

  viewFile(url: string) {
    window.open(url, '_blank');
  }

  goToContractDetail(id: string) {
    const url = `/document/contract/${id}`;
    window.open(url, '_blank');
  }

  goToContractorDetail(id: string) {
    const url = `/document/contractor/${id}`;
    window.open(url, '_blank');
  }
}
