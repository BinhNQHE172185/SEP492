import { Component, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CalendarModule } from 'primeng/calendar';
import { ContractApiService } from '../../../../apis/contractAPIs/contract-api.service';
import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-contract-detail',
  imports: [
    CalendarModule,
    FormsModule,
    DialogModule,
    CommonModule,
    ButtonModule,
    ToastModule,

  ],
  templateUrl: './contract-detail.component.html',
  styleUrl: './contract-detail.component.scss',
  providers: [ConfirmationService, MessageService]
})
export class ContractDetailComponent {
  @Input() contractId!: string;
  @Input() displayDetailDialog: boolean = false;
  @Output() closeDialogEvent = new EventEmitter<void>();

  uploadedFiles: any[] = [];
  file: any;
  calendarValue: any = null;
  contract: any;

  constructor(
    private messageService: MessageService,
    private contractService: ContractApiService
  ) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['contractId'] && this.contractId) {
      this.loadContractDetail();
    }
  }

  loadContractDetail() {
    if (!this.contractId) return;

    this.contractService.getContractDetail(this.contractId).subscribe(
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

  goToProposalDetail(id: string) {
    const url = `/document/report/${id}`;
    window.open(url, '_blank');
  }

  goToContractorDetail(id: string) {
    const url = `/document/contractor/${id}`;
    window.open(url, '_blank');
  }
}
