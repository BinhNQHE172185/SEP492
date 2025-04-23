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
import { TableModule } from 'primeng/table';
import { SelectModule } from 'primeng/select';
import { OpenAIApiService } from '../../../../apis/openAIAPIs/openAI-api';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

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
    InputNumberModule,
    TableModule,
    SelectModule,
    ProgressSpinnerModule,
    SelectModule
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
  prompt: string = `Hãy phân tích nội dung biên bản nghiệm thu và trích xuất các thông tin`
  isLoading: boolean = false;

  report: any;
  contract: { contractId: string; contractValue: string }[] = [];

  constructor(
    private messageService: MessageService,
    private acceptanceService: AcceptanceRecordApiService,
    private contractService: ContractApiService,
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

  toggleCalculationPanel() {
    this.showCalculationPanel = !this.showCalculationPanel;
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
    if (this.report.contractId === '') {
      this.messageService.add({
        severity: 'warn',
        summary: 'Cảnh báo',
        detail: 'Vui lòng chọn hợp đồng.'
      });
      return;
    }



    const reportData = new FormData();
    reportData.append("title", this.report.title);
    reportData.append("contractId", this.report.contractId);
    reportData.append("finalPrice", this.report.finalPrice);
    reportData.append("acceptanceDate", this.report.acceptanceDate.toLocaleDateString('en-CA'));
    reportData.append("file", this.file);

    if (this.selectedId) {
      this.isLoading = true;
      this.acceptanceService.updateAcceptanceRecord(this.selectedId, reportData).subscribe(
        (response) => {
          this.messageService.add({ severity: 'success', summary: 'Thành công', detail: response.message });
          this.closeDialog();
          this.isLoading = false;
        },
        (error) => {
          this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: error.error.message });
          this.isLoading = false;
        }
      );
    } else {
      if (this.file === null || this.file === undefined) {
        this.messageService.add({
          severity: 'warn',
          summary: 'Cảnh báo',
          detail: 'Vui lòng chọn file.'
        });
        return;
      }

      this.messageService.add({ severity: 'info', summary: 'Đang tải', detail: 'Vui lòng chờ.' });
      this.isLoading = true;
      this.acceptanceService.createAcceptanceRecord(reportData).subscribe(
        (response) => {
          this.messageService.add({ severity: 'success', summary: 'Thành công', detail: response.message });
          this.closeDialog();
          this.isLoading = false;
        },
        (error) => {
          this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: error.error.message });
          this.isLoading = false;
        }
      );
    }
  }

  onFileSelect(event: any) {
    if (event.files && event.files.length > 0) {
      this.uploadedFiles = event.files[0];
      this.file = event.files[0]
    }

    const isAllFieldsEmpty =
      !this.report.title &&
      !this.report.contractId;

    if (isAllFieldsEmpty) {
      this.isLoading = true;
      this.messageService.add({
        severity: 'info',
        summary: 'Đang tải',
        detail: 'Vui lòng chờ.'
      });

      this.openAIService.analyzeUploadRecordFile(this.file, this.prompt).subscribe({
        next: (response) => {
          const result = response.result;
          this.report.title = result.title;
          this.report.contractId = result.contractId;
          this.report.finalPrice = result.finalPrice;
          this.report.acceptanceDate = new Date(result.acceptanceDate);

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
          this.isLoading = false;
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
    this.report = {
      title: '',
      contract: '',
      finalPrice: '',
      acceptanceDate: new Date(),
    };
    this.file = null;
    this.selectedItems = [
      { selected: null, quantity: 1, isNew: 'new', calculatedValue: 0 }
    ];
    this.showCalculationPanel = false;
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
