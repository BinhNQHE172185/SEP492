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

interface Report {
  date: string;
  author: string;
  title: string;
  content: string;
  fileName?: string;
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
    FormsModule
  ],
  templateUrl: './list-report.component.html',
  styleUrls: ['./list-report.component.scss']
})
export class ListReportComponent implements OnInit {
  // --------------------------
  // Mock Data
  // --------------------------
  reports: Report[] = [
    {
      date: '2024-01-15',
      author: 'Nguyễn Văn A',
      title: 'Đề xuất nâng cấp hệ thống phần mềm quản lý',
      content: 'Đề xuất nâng cấp hệ thống phần mềm quản lý',
      fileName: 'detrinh_nangcap_v1.pdf'
    },
    {
      date: '2023-08-10',
      author: 'Trần Thị B',
      title: 'Tờ trình xin phê duyệt chiến lược marketing mới',
      content: 'Tờ trình xin phê duyệt chiến lược marketing mới'
    },
    {
      date: '2024-07-17',
      author: 'Phạm Thị D',
      title: 'Đề xuất tổ chức sự kiện tri ân khách hàng',
      content: 'Đề xuất tổ chức sự kiện tri ân khách hàng',
      fileName: 'totrinh_sukien.pdf'
    }
  ];

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

  // --------------------------
  // Search
  // --------------------------
  searchKey = '';
  filteredReports: Report[] = [];

  ngOnInit() {
    // Initialize filtered list
    this.filteredReports = [...this.reports];
  }

  // --------------------------
  // Search Handler
  // --------------------------
  onSearchChange() {
    const key = this.searchKey.toLowerCase().trim();
    this.filteredReports = this.reports.filter((report) =>
      report.date.toLowerCase().includes(key) ||
      report.author.toLowerCase().includes(key) ||
      report.title.toLowerCase().includes(key) ||
      report.content.toLowerCase().includes(key)
    );
  }

  // --------------------------
  // Open Dialogs
  // --------------------------
  openAddDialog() {
    // Reset fields
    this.addDate = null;
    this.addAuthor = '';
    this.addTitle = '';
    this.addContent = '';
    this.addFileName = '';
    this.displayAddDialog = true;
  }

  openDetailDialog(report: Report) {
    // Show detail data
    this.detailReport = report;
    this.displayDetailDialog = true;
  }

  openEditDialog(index: number) {
    const report = this.filteredReports[index];
    this.editIndex = index;
    // Convert string date -> Date object (if needed)
    // For demonstration, we assume the date is in YYYY-MM-DD format
    const parts = report.date.split('-'); // [YYYY, MM, DD]
    this.editDate = new Date(
      parseInt(parts[0], 10),
      parseInt(parts[1], 10) - 1,
      parseInt(parts[2], 10)
    );
    this.editAuthor = report.author;
    this.editTitle = report.title;
    this.editContent = report.content;
    this.editFileName = report.fileName || '';
    this.displayEditDialog = true;
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
    // Convert Date -> string (YYYY-MM-DD)
    const dateString = this.addDate
      ? `${this.addDate.getFullYear()}-${(this.addDate.getMonth() + 1)
          .toString()
          .padStart(2, '0')}-${this.addDate.getDate().toString().padStart(2, '0')}`
      : '';

    const newReport: Report = {
      date: dateString,
      author: this.addAuthor,
      title: this.addTitle,
      content: this.addContent,
      fileName: this.addFileName
    };

    this.reports.push(newReport);
    this.onSearchChange(); // refresh filtered data
    this.displayAddDialog = false;
  }

  // --------------------------
  // Edit Dialog Save
  // --------------------------
  saveEditedReport() {
    if (this.editIndex < 0 || this.editIndex >= this.filteredReports.length) {
      return;
    }
    const dateString = this.editDate
      ? `${this.editDate.getFullYear()}-${(this.editDate.getMonth() + 1)
          .toString()
          .padStart(2, '0')}-${this.editDate.getDate().toString().padStart(2, '0')}`
      : '';

    // Update the original array item
    const globalIndex = this.reports.indexOf(this.filteredReports[this.editIndex]);
    this.reports[globalIndex] = {
      date: dateString,
      author: this.editAuthor,
      title: this.editTitle,
      content: this.editContent,
      fileName: this.editFileName
    };

    // Re-filter after edit
    this.onSearchChange();
    this.displayEditDialog = false;
  }

  // --------------------------
  // Delete
  // --------------------------
  deleteReport(index: number) {
    const globalIndex = this.reports.indexOf(this.filteredReports[index]);
    this.reports.splice(globalIndex, 1);
    this.onSearchChange();
  }

  // --------------------------
  // File Upload Handlers
  // --------------------------
  onAddFileSelect(event: any) {
    // For demonstration, we just take the file name
    const file = event.files[0];
    this.addFileName = file.name;
  }

  onEditFileSelect(event: any) {
    const file = event.files[0];
    this.editFileName = file.name;
  }

  downloadFile(fileName: string) {
    // In real usage, you'd handle actual file download
    alert(`Tải xuống: ${fileName}`);
  }
}