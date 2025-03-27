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
import { ContractApiService } from '../../../../apis/contractAPIs/contract-api.service';
import { BudgetApiService } from '../../../../apis/budgetProposalAPIs/budget-api.service';
import { DropdownModule } from 'primeng/dropdown';
import { ContractorApiService } from '../../../../apis/contractorAPIs/contractor-api.service';
import { InputNumberModule } from 'primeng/inputnumber';

@Component({
  selector: 'app-contract-create-edit',
  imports: [
    DialogModule,
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
  templateUrl: './contract-create-edit.component.html',
  styleUrl: './contract-create-edit.component.scss',
  providers: [ConfirmationService, MessageService]
})
export class ContractCreateEditComponent implements OnChanges {
  @Input() displayAddDialog: boolean = false;
  @Input() selectedContractId: string | null = null;
  @Output() closeDialogEvent = new EventEmitter<void>();

  uploadedFiles: any[] = [];
  file: any;
  calendarValue: any = null;

  contract: any;
  budgetProposal: any;
  contractor: any;

  constructor(
    private messageService: MessageService,
    private contractService: ContractApiService,
    private budgetProposalService: BudgetApiService,
    private contractorService: ContractorApiService
  ) { }

  loadData() {
    this.budgetProposalService.getBudgetList().subscribe(
      (response) => {
        if (response) {
          this.budgetProposal = response;
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
    this.contractorService.getContractorsList().subscribe(
      (response) => {
        if (response) {
          this.contractor = response;
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
    if (this.selectedContractId) {
      this.contractService.getContractDetail(this.selectedContractId).subscribe(
        (response) => {
          if (response) {
            this.contract = response;
            this.contract.startDate = new Date(this.contract.startDate);
            this.contract.endDate = new Date(this.contract.endDate);
            this.file = this.contract.url;
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
    const reportData = new FormData();
    reportData.append("proposalId", this.contract.proposalId!);
    reportData.append("contractorId", this.contract.contractorId!);
    reportData.append("title", this.contract.title);
    reportData.append("contractValue", this.contract.contractValue);
    reportData.append("startDate", this.contract.startDate.toISOString().split("T")[0]);
    reportData.append("endDate", this.contract.endDate.toISOString().split("T")[0]);
    reportData.append("file", this.file);

    if (this.selectedContractId) {
      this.contractService.updateContract(this.selectedContractId, reportData).subscribe(
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
      this.contractService.createContract(reportData).subscribe(
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
    this.contract = {
      courseCode: '',
      courseName: '',
      startDate: new Date(),
      endDate: new Date(),
      contractor: '',
      cost: '',
      contractNo: '',
      contractDate: new Date(),
      content: ''
    };
    this.file = '';
  }

  closeDialog() {
    this.selectedContractId = '';
    this.displayAddDialog = false;
    this.closeDialogEvent.emit();
  }
}
