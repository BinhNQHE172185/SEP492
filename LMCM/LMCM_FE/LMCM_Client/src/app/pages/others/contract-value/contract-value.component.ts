import { Component } from '@angular/core';
import { ContractValueApiComponent } from '../../../apis/contractValueAPIs/contract-value-api.servcie';
import { TableModule } from 'primeng/table';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { ConfirmationService, MessageService } from 'primeng/api';
import { TagModule } from 'primeng/tag';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { MessageModule } from 'primeng/message';

@Component({
  selector: 'app-contract-value',
  imports: [
    TableModule,
    CardModule,
    ButtonModule,
    ToastModule,
    TagModule,
    ProgressSpinnerModule,
    CommonModule,
    FormsModule,
    InputNumberModule,
    InputTextModule,
    TextareaModule,
    ConfirmDialogModule,
    DialogModule,
    MessageModule
  ],
  standalone: true,
  templateUrl: './contract-value.component.html',
  styleUrl: './contract-value.component.scss',
  providers: [
    MessageService,
    ConfirmationService
  ]
})
export class ContractValueComponent {
  contractValues: any[] = [];
  displayAddDialog: boolean = false;
  isLoading: boolean = false;
  newContractValue: any = {
    category: '',
    measurementUnit: '',
    standardRate: null,
    contractValue: null,
    qualityRequirements: '',
    valueId: null,
  };

  constructor(
    private contractValueService: ContractValueApiComponent,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) { }

  ngOnInit() {
    this.loadData();
  }
  loadData() {
    this.contractValueService.getContractValue().subscribe(
      (response) => {
        this.contractValues = response;
      },
      (error) => {
        this.messageService.add({
          severity: 'error',

        }
        )
      });
  }


  updateContractValue() {
    const request = this.contractValues.map(item => ({
      valueId: item.valueId,
      category: item.category,
      measurementUnit: item.measurementUnit,
      standardRate: item.standardRate,
      qualityRequirements: item.qualityRequirements,
      contractValue: item.contractValue,
      valueRatioForUpdate: item.valueRatioForUpdate
    }));
    this.isLoading = true;
    this.contractValueService.updateContractValue(request).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.loadData();
        this.messageService.add({
          severity: 'success',
          summary: 'Thành công',
          detail: 'Cập nhật thành công',
        })
      },
      error: (err) => {
        this.isLoading = false;
        this.messageService.add({
          severity: 'error',
          summary: 'Thất bại',
          detail: err.error.message,
        })
      }
    });
  }

  showAddDialog() {
    this.displayAddDialog = true;
  }

  closeAddDialog() {
    this.displayAddDialog = false;
    this.resetNewContractValue();
  }

  resetNewContractValue() {
    this.newContractValue = {
      category: '',
      measurementUnit: '',
      standardRate: null,
      contractValue: null,
      qualityRequirements: ''
    };
    console.log(this.newContractValue);
  }

  saveContractValue() {
    if (this.isFormValid()) {
      this.contractValues.push({ ...this.newContractValue });
      this.updateContractValue();
      this.closeAddDialog();
    } else {
      this.messageService.add({
        severity: 'warn',
        summary: 'Cảnh báo',
        detail: 'Vui lòng điền đầy đủ thông tin trước khi lưu.',
      });
    }
  }

  isFormValid() {
    return this.newContractValue.category && this.newContractValue.measurementUnit && this.newContractValue.contractValue;
  }

  deleteContractValue(item: any) {
    this.confirmationService.confirm({
      header: 'Xóa dữ liệu',
      message: 'Bạn có chắc chắn muốn xóa? Hành động này là không thể hoàn tác.',
      acceptLabel: 'Đồng ý',
      rejectLabel: 'Hủy',
      accept: () => {
        this.isLoading = true;
        const index = this.contractValues.findIndex(c => c.valueId === item.valueId);
        if (index > -1) {
          this.contractValues.splice(index, 1);
        }
        this.updateContractValue();
      }
    });
  }
}
