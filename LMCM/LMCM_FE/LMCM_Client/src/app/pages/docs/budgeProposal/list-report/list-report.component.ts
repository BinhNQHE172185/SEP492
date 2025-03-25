import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputGroupModule } from 'primeng/inputgroup';
import { CardModule } from 'primeng/card';
import { FormsModule } from '@angular/forms';
import { TagModule } from 'primeng/tag';
import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { FileUploadModule } from 'primeng/fileupload';
import { TextareaModule } from 'primeng/textarea';
import { CalendarModule } from 'primeng/calendar';
import { Subscription } from 'rxjs';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ReportDetailComponent } from '../report-detail/report-detail.component';
import { BudgetApiService } from '../../../../apis/budgetProposalAPIs/budget-api.service';
import { searchService } from '../../../service/search/search-service.service';

interface Report {
  proposalDate: string;
  authorId: string;
  title: string;
  file: File;
}

interface PagingRequest {
  searchKey?: string;
  pageIndex: number;
  pageSize: number;
}

@Component({
  selector: 'app-list-report',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    InputGroupModule,
    CardModule,
    ConfirmDialogModule,
    ToastModule,
    FileUploadModule,
    DialogModule,
    FormsModule,
    TagModule,
    TextareaModule,
    CalendarModule,
    FormsModule,
    ReportDetailComponent
  ],
  templateUrl: './list-report.component.html',
  styleUrls: ['./list-report.component.scss'],
  providers: [
    MessageService,
    ConfirmationService
  ]
})
export class ListReportComponent implements OnInit {
  reports: any = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  searchKey = '';

  selectedFile: File | null = null;

  private searchSubscription!: Subscription;

  constructor(private reportService: BudgetApiService,
    private searchService: searchService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService) { }

  // --------------------------
  // Dialog States
  // --------------------------
  displayAddDialog = false;
  displayDetailDialog = false;
  displayEditDialog = false;

  // --------------------------
  // Add Dialog Fields
  // --------------------------
  addDate: Date | null = null;
  addAuthor = '';
  addTitle = '';
  addContent = '';
  addFileName = '';

  // --------------------------
  // Edit Dialog Fields
  // --------------------------
  editDate: Date | null = null;
  editAuthor = '';
  editTitle = '';
  editContent = '';
  editFileName = '';
  editIndex = -1; // Keep track of which item we are editing

  // --------------------------
  // Detail Dialog Fields
  // --------------------------
  detailReport: Report | null = null;

  ngOnInit() {
    // Initialize filtered list
    this.searchSubscription = this.searchService.searchQuery$.subscribe(
      (query) => {
        this.searchKey = query;
        this.loadBudget();
      }
    );
  }

  loadBudget(event?: any) {
    if (event) {
      this.pageNumber = Math.floor(event.first / event.rows) + 1;
      this.pageSize = event.rows;
    }

    const request: PagingRequest = {
      pageIndex: this.pageNumber,
      pageSize: this.pageSize,
      searchKey: this.searchKey,
    };
    this.reportService.getBudget(request).subscribe(
      (response) => {
        this.reports = response.items;
        this.totalCount = response.totalCount;
      },
      (error) => {
        console.error("Lỗi khi tải danh sách môn học:", error);
      }
    );
  }

  ngOnDestroy(): void {
    if (this.searchSubscription) {
      this.searchService.updateSearchQuery('');
      this.searchSubscription.unsubscribe();
    }
  }

  onSearchChange(query: string) {
    this.searchService.updateSearchQuery(query);
  }

  // --------------------------
  // Open Dialogs
  // --------------------------
  openAddDialog() {
    // Reset fields
    this.addDate = null;
    this.addAuthor = '';
    this.addTitle = '';
    this.addFileName = '';
    this.displayAddDialog = true;
  }

  openDetailDialog(report: Report) {
    // Show detail data
    this.detailReport = report;
    this.displayDetailDialog = true;
  }

  openEditDialog(index: number) {
    // const report = this.filteredReports[index];
    // this.editIndex = index;
    // // Convert string date -> Date object (if needed)
    // // For demonstration, we assume the date is in YYYY-MM-DD format
    // const parts = report.date.split('-'); // [YYYY, MM, DD]
    // this.editDate = new Date(
    //   parseInt(parts[0], 10),
    //   parseInt(parts[1], 10) - 1,
    //   parseInt(parts[2], 10)
    // );
    // this.editAuthor = report.author;
    // this.editTitle = report.title;
    // this.editContent = report.content;
    // this.editFileName = report.fileName || '';
    // this.displayEditDialog = true;
  }

  // --------------------------
  // Close Dialogs
  // --------------------------
  closeDialog(dialogType: string) {
    if (dialogType === 'add') {
      this.displayAddDialog = false;
    } else if (dialogType === 'detail') {
      this.displayDetailDialog = false;
    } else if (dialogType === 'title') {
      this.displayDetailDialog = false;
    } else if (dialogType === 'edit') {
      this.displayEditDialog = false;
    }
  }

  // --------------------------
  // Add Dialog Save
  // --------------------------
  saveNewReport() {
    if (!this.selectedFile) {
      console.error('Vui lòng chọn file trước khi lưu.');
      return;
    }
    // Convert Date -> string (YYYY-MM-DD)
    const dateString = this.addDate
      ? `${this.addDate.getFullYear()}-${(this.addDate.getMonth() + 1)
        .toString()
        .padStart(2, '0')}-${this.addDate.getDate().toString().padStart(2, '0')}`
      : '';

    const newReport: Report = {
      proposalDate: dateString,
      authorId: this.addAuthor,
      title: this.addTitle,
      file: this.selectedFile
    };

    this.reportService.createBudget(newReport).subscribe(
      (response) => {
        this.displayAddDialog = false;
      },
      (error) => {
        console.error("Lỗi khi tải danh sách môn học:", error);
      }
    );
  }

  // --------------------------
  // Edit Dialog Save
  // --------------------------
  saveEditedReport() {
    // if (this.editIndex < 0 || this.editIndex >= this.filteredReports.length) {
    //   return;
    // }
    // const dateString = this.editDate
    //   ? `${this.editDate.getFullYear()}-${(this.editDate.getMonth() + 1)
    //     .toString()
    //     .padStart(2, '0')}-${this.editDate.getDate().toString().padStart(2, '0')}`
    //   : '';

    // // Update the original array item
    // const globalIndex = this.reports.indexOf(this.filteredReports[this.editIndex]);
    // this.reports[globalIndex] = {
    //   date: dateString,
    //   author: this.editAuthor,
    //   title: this.editTitle,
    //   content: this.editContent,
    //   fileName: this.editFileName
    // };

    // // Re-filter after edit
    // this.onSearchChange();
    // this.displayEditDialog = false;
  }

  // --------------------------
  // Delete
  // --------------------------
  deleteReport(index: number) {
    // const globalIndex = this.reports.indexOf(this.filteredReports[index]);
    // this.reports.splice(globalIndex, 1);
    // this.onSearchChange();
  }

  // --------------------------
  // File Upload Handlers
  // --------------------------
  onAddFileSelect(event: any) {
    this.selectedFile = event.files[0];
  }

  onEditFileSelect(event: any) {
    const file = event.files[0];
    this.editFileName = file.name;
  }

  downloadFile(fileName: File) {
    // In real usage, you'd handle actual file download
    alert(`Tải xuống: ${fileName}`);
  }
}