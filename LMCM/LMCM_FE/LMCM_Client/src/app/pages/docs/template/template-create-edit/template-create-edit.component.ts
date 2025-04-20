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
import { DocumentTemplateApiService } from '../../../../apis/templateAPIs/template-api.service';
import { TemplateStatus } from '../../../../../shared/Constants/StatusConstants';

@Component({
  selector: 'app-template-create-edit',
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
  ],
  providers: [ConfirmationService, MessageService],
  standalone: true,
  templateUrl: './template-create-edit.component.html',
  styleUrl: './template-create-edit.component.scss'
})
export class TemplateCreateEditComponent implements OnChanges {
  @Input() displayAddDialog: boolean = false;
  @Input() selectedId: string | null = null;
  @Output() closeDialogEvent = new EventEmitter<void>();

  uploadedFiles: any[] = [];
  file: any;
  calendarValue: any = null;

  template: any;
  budgetProposal: any;
  contractor: any;

  constructor(
    private messageService: MessageService,
    private templateService: DocumentTemplateApiService,

  ) { }
  statusList = Object.keys(TemplateStatus)

    .map(key => ({
      label: key,
      value: TemplateStatus[key as keyof typeof TemplateStatus]
    }));


  ngOnChanges() {
    console.log("Selected ID:", this.selectedId);


    if (this.selectedId) {
      console.log("Selected ID:", this.selectedId);
      this.templateService.getTemplateDetail(this.selectedId).subscribe(
        (response) => {
          if (response) {
            this.template = response;

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
    if (!this.template.templateName) {
      this.messageService.add({ severity: 'warn', summary: 'Thất bại', detail: 'Vui lòng nhập tên mẫu tài liệu.' });
      return;
    }

    const reportData = new FormData();
    reportData.append("templateName", this.template.templateName!);
    reportData.append("templateType", this.template.templateType!);
    reportData.append("file", this.file);

    if (this.selectedId) {
      this.templateService.updateTemplate(this.selectedId, reportData).subscribe(
        (response) => {
          this.messageService.add({ severity: 'success', summary: 'Thành công', detail: response.message });
          this.closeDialog();
        },
        (error) => {
          this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: error.error.message });
        }
      );
    } else {
      if (!this.file) {
        this.messageService.add({ severity: 'warn', summary: 'Thất bại', detail: 'Vui lòng tải lên tệp.' });
        return;
      }

      this.messageService.add({ severity: 'info', summary: 'Đang tải', detail: 'Vui lòng chờ.' });
      this.templateService.createTemplate(reportData).subscribe(
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
    this.template = {
      templateName: '',
      templateType: '',
      templateStatus: this.statusList.length ? this.statusList[0].value : '', // Gán giá trị mặc định
    };
    this.file = null; // Đảm bảo không bị null
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
