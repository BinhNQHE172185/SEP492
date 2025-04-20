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
import { TableModule } from 'primeng/table';
import { SelectModule } from 'primeng/select';
import { OpenAIApiService } from '../../../../apis/openAIAPIs/openAI-api';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

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
    InputNumberModule,
    TableModule,
    SelectModule,
    ProgressSpinnerModule
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
  contractValueList: any[] = [];
  price: any[] = [];
  selectedItems: {
    selected: {
      standardRate?: number;
      valueRatioForUpdate?: number;
      contractValue?: number;
      measurementUnit?: string;
    } | null;
    quantity: number;
    isNew: string | null;
    calculatedValue: number;
  }[] = [
      { selected: null, quantity: 1, isNew: 'new', calculatedValue: 0 }
    ];
  totalValue: number = 0;

  showCalculationPanel: boolean = false;
  prompt: string = `Hãy phân tích nội dung hợp đồng và trích xuất các thông tin`
  isLoading: boolean = false;

  constructor(
    private messageService: MessageService,
    private contractService: ContractApiService,
    private budgetProposalService: BudgetApiService,
    private contractorService: ContractorApiService,
    private openAIService: OpenAIApiService,
  ) { }
  loadData() {
    this.contractService.getContractValue().subscribe(
      (response) => {
        if (response) {
          this.price = response;
          this.contractValueList = response.map((item: any) => ({
            ...item,
            selected: false
          }));
        }
      },
      (error) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Thất bại',
          detail: error?.error?.message || 'Đã xảy ra lỗi khi tải dữ liệu.'
        });
      }
    )
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

  addRow() {
    this.selectedItems.push({
      selected: null,
      quantity: 0,
      isNew: 'new',
      calculatedValue: 0
    });
  }

  removeRow(index: number) {
    this.selectedItems.splice(index, 1);
    this.updateTotalValue();
  }

  updateTotalValue() {
    let total = 0;

    this.selectedItems.forEach((row) => {
      const item = row.selected;

      const quantity = Number(row.quantity);
      const isValidQuantity = !isNaN(quantity) && quantity > 0;

      if (item && isValidQuantity) {
        const standardRate = Number(item.standardRate) || 0;
        const ratio = Number(item.valueRatioForUpdate) || 1;
        const value = Number(item.contractValue) || 1;

        if (row.isNew === 'new') {
          row.calculatedValue = value * quantity * standardRate;
        } else if (row.isNew === 'adjust') {
          row.calculatedValue = value * quantity * standardRate * ratio;
        } else {
          row.calculatedValue = 0;
        }
      } else {
        row.calculatedValue = 0;
      }

      total += row.calculatedValue;
    });

    this.totalValue = total;
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
    if (this.contract.startDate > this.contract.endDate) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Cảnh báo',
        detail: 'Ngày bắt đầu không được lớn hơn ngày kết thúc.'
      });
      return;
    }

    const reportData = new FormData();
    reportData.append("proposalId", this.contract.proposalId!);
    reportData.append("contractorId", this.contract.contractorId!);
    reportData.append("title", this.contract.title);
    reportData.append("contractValue", this.contract.contractValue);
    reportData.append("startDate", this.contract.startDate.toLocaleDateString('en-CA'));
    reportData.append("endDate", this.contract.endDate.toLocaleDateString('en-CA'));
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

  toggleCalculationPanel() {
    this.showCalculationPanel = !this.showCalculationPanel;
  }

  onFileSelect(event: any) {
    if (event.files && event.files.length > 0) {
      this.uploadedFiles = event.files[0];
      this.file = event.files[0];
    }

    const isAllFieldsEmpty =
      !this.contract.title &&
      !this.contract.contractorId &&
      !this.contract.contractValue &&
      !this.contract.proposalId;

    if (isAllFieldsEmpty) {
      this.isLoading = true;
      this.messageService.add({
        severity: 'info',
        summary: 'Đang tải',
        detail: 'Vui lòng chờ.'
      });

      this.openAIService.analyzeUploadFile(this.file, this.prompt).subscribe({
        next: (response) => {
          const result = response.result;
          this.contract.title = result.title;
          this.contract.contractorId = result.contractorId;
          this.contract.contractValue = result.contractValue;
          this.contract.contractNo = result.contractNo;
          this.contract.startDate = new Date(result.startDate);
          this.contract.endDate = new Date(result.endDate);
          this.contract.contractDate = new Date(result.contractDate);

          this.messageService.add({
            severity: 'success',
            summary: 'Thành công',
            detail: response.message
          });
        },
        error: (error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Thất bại',
            detail: error?.error?.message || 'Có lỗi xảy ra. Vui lòng thử lại.'
          });
        },
        complete: () => {
          this.isLoading = false;
        }
      });
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
    this.selectedItems = [
      { selected: null, quantity: 1, isNew: 'new', calculatedValue: 0 }
    ];
    this.showCalculationPanel = false;
  }

  closeDialog() {
    this.selectedContractId = '';
    this.displayAddDialog = false;
    this.closeDialogEvent.emit();
  }
  viewFile(url: string) {
    window.open(url, '_blank');
  }
}
