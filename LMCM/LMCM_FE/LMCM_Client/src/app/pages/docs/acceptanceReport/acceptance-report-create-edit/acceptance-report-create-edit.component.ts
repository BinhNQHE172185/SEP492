import { Component, EventEmitter, Input, OnChanges, Output } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FileUploadModule } from 'primeng/fileupload';
import { DatePickerModule } from 'primeng/datepicker';
import { ToastModule } from 'primeng/toast';
import { DropdownModule } from 'primeng/dropdown';
import { InputNumberModule } from 'primeng/inputnumber';
import { AcceptanceRecordApiService } from '../../../../apis/acceptanceRecordAPIs/acceptance-api.service';
import { ContractApiService } from '../../../../apis/contractAPIs/contract-api.service';

@Component({
  selector: 'app-acceptance-report-create-edit',
  imports: [DialogModule,
    InputTextModule,
    ButtonModule,
    CommonModule,
    FormsModule,
    FileUploadModule,
    DatePickerModule,
    ToastModule,
    DropdownModule,
    InputNumberModule
  ],
  providers: [ConfirmationService, MessageService],
  standalone: true,
  templateUrl: './acceptance-report-create-edit.component.html',
  styleUrl: './acceptance-report-create-edit.component.scss'
})
export class AcceptanceReportCreateEditComponent implements OnChanges {
  @Input() displayAddDialog: boolean = false;
  @Input() selectedId: string | null = null;
  @Output() closeDialogEvent = new EventEmitter<void>();

  uploadedFiles: any[] = [];
  file: any;
  calendarValue: any = null;

  report: any;
  contract: { contractId: string; contractValue: string }[] = [];

  constructor(
    private messageService: MessageService,
    private acceptanceService: AcceptanceRecordApiService,
    private contractService: ContractApiService,
  ) { }

  loadData() {
    this.contractService.getContractList().subscribe(
      (response) => {
        if (response) {
          this.contract = response;
        }
      },
      (error) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Thất bại',
          detail: error?.error?.message || 'Đã xảy ra lỗi khi tải dữ liệu.'
        });
      }
    );
  }

  ngOnChanges() {
    this.loadData();
    if (this.selectedId) {
      this.acceptanceService.getAcceptanceRecordDetail(this.selectedId).subscribe(
        (response) => {
          if (response) {
            this.report = response;
            this.report.acceptanceDate = new Date(this.report.acceptanceDate);
            this.file = this.report.url;
          }
        },
        (error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Thất bại',
            detail: error?.error?.message || 'Đã xảy ra lỗi khi tải dữ liệu.'
          });
        }
      );
    } else {
      this.resetForm();
    }
  }

  save() {
    const selectedContract = this.contract.find((c: { contractId: string; contractValue: string }) => c.contractId === this.report.contractId);

    if (selectedContract) {
      const contractPrice = parseFloat(selectedContract.contractValue);
      const finalPrice = parseFloat(this.report.finalPrice);

      if (finalPrice > contractPrice) {
        this.messageService.add({
          severity: 'warn',
          summary: 'Cảnh báo',
          detail: 'Giá trị nghiệm thu đang lớn hơn giá trị hợp đồng đã chọn.'
        });
      }
    }

    const reportData = new FormData();
    reportData.append("title", this.report.title);
    reportData.append("contractId", this.report.contractId);
    reportData.append("finalPrice", this.report.finalPrice);
    reportData.append("acceptanceDate", this.report.acceptanceDate.toLocaleDateString('en-CA'));
    reportData.append("file", this.file);

    if (this.selectedId) {
      this.acceptanceService.updateAcceptanceRecord(this.selectedId, reportData).subscribe(
        (response) => {
          this.messageService.add({ severity: 'success', summary: 'Thành công', detail: response.message });
          this.closeDialog();
        },
        (error) => {
          this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: error.error.message });
        }
      );
    } else {
      this.messageService.add({ severity: 'info', summary: 'Đang tải', detail: 'Vui lòng chờ.' });
      this.acceptanceService.createAcceptanceRecord(reportData).subscribe(
        (response) => {
          this.messageService.add({ severity: 'success', summary: 'Thành công', detail: response.message });
          this.closeDialog();
        },
        (error) => {
          this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: error.error.message });
        }
      );
    }
  }

  onFileSelect(event: any) {
    if (event.files && event.files.length > 0) {
      this.uploadedFiles = event.files[0];
      this.file = event.files[0]
    }
  }

  downloadFile(url: string) {
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', 'file');
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }

  resetForm() {
    this.report = {
      title: '',
      contract: '',
      finalPrice: '',
      acceptanceDate: new Date(),
    };
    this.file = null;
  }

  closeDialog() {
    this.selectedId = '';
    this.displayAddDialog = false;
    this.closeDialogEvent.emit();
  }
  viewFile(url: string) {
    window.open(url, '_blank');
  }
}
