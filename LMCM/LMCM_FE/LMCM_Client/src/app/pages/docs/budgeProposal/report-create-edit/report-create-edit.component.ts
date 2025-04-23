import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { DialogModule } from 'primeng/dialog';
import { FileUploadModule } from 'primeng/fileupload';
import { InputTextModule } from 'primeng/inputtext';
import { BudgetApiService } from '../../../../apis/budgetProposalAPIs/budget-api.service';
import { DatePickerModule } from 'primeng/datepicker';
import { ToastModule } from 'primeng/toast';
import { DropdownModule } from 'primeng/dropdown';
import { InputNumberModule } from 'primeng/inputnumber';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { MAX_TODAY_DATE, MIN_COMPLETION_DATE } from '../../../../../shared/Constants/DateConstants';

@Component({
  selector: 'app-report-create-edit',
  standalone: true,
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
    ProgressSpinnerModule
  ],
  templateUrl: './report-create-edit.component.html',
  styleUrl: './report-create-edit.component.scss',
  providers: [ConfirmationService, MessageService]
})
export class ReportCreateEditComponent {
  @Input() displayAddDialog: boolean = false;
  @Input() selectedReportId: string | null = null;
  @Output() closeDialogEvent = new EventEmitter<void>();

  uploadedFiles: any[] = [];
  file: any;
  calendarValue: any = null;

  minCompletionDate = MIN_COMPLETION_DATE;
  maxCompletionDate = MAX_TODAY_DATE;

  constructor(
    private messageService: MessageService,
    private reportService: BudgetApiService
  ) { }

  addDate: Date | null = null;
  addAuthor: string = '';
  addTitle: string = '';
  addFileName: string = '';

  isLoading: boolean = false;
  report: any;

  ngOnChanges() {
    if (this.selectedReportId) {
      this.reportService.getBudgetDetail(this.selectedReportId).subscribe(
        (response) => {
          this.report = response;
          this.addTitle = this.report.title;
          this.addAuthor = this.report.author.name;
          this.addDate = new Date(this.report.proposalDate);
          this.file = this.report.downloadUrl;
        },
        (error) => {
          this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: error.error.message });
        }
      );
    } else {
      this.resetForm();
    }
  }

  resetForm() {
    this.addTitle = "";
    this.addAuthor = "";
    this.addDate = null;
    this.file = '';
  }

  saveReport() {

    const reportData = new FormData();
    reportData.append("title", this.addTitle);
    reportData.append("proposalDate", this.addDate ? this.addDate.toLocaleDateString('en-CA') : '');
    reportData.append("file", this.file);

    if (this.selectedReportId) {
      this.isLoading = true;
      this.reportService.updateBudget(this.selectedReportId, reportData).subscribe(
        (response) => {
          this.messageService.add({ severity: 'success', summary: 'Thành công', detail: response.message });
          this.closeDialog();
          this.isLoading = false;
        },
        (error) => {
          const errors = error.error.errors;
          const allMessages = [];

          for (const key in errors) {
            if (errors.hasOwnProperty(key)) {
              allMessages.push(...errors[key]);
            }
          }

          allMessages.forEach(msg => {
            this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: msg });
          });
          this.isLoading = false;
        }
      );
    } else {
      this.messageService.add({ severity: 'info', summary: 'Đang tải', detail: 'Vui lòng chờ.' });
      this.isLoading = true;
      this.reportService.createBudget(reportData).subscribe(
        (response) => {
          this.messageService.add({ severity: 'success', summary: 'Thành công', detail: response.message });
          this.closeDialog();
          this.isLoading = false;
        },
        (error) => {
          const errors = error.error.errors;
          const allMessages = [];

          for (const key in errors) {
            if (errors.hasOwnProperty(key)) {
              allMessages.push(...errors[key]);
            }
          }

          allMessages.forEach(msg => {
            this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: msg });
          });
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
    this.selectedReportId = '';
    this.displayAddDialog = false;
    this.closeDialogEvent.emit();
  }
  viewFile(url: string) {
    window.open(url, '_blank');
  }
}
