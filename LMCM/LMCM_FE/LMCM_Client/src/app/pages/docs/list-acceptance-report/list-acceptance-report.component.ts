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

@Component({
  selector: 'app-list-acceptance-report',
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
    CalendarModule
  ],
  templateUrl: './list-acceptance-report.component.html',
  styleUrls: ['./list-acceptance-report.component.scss']
})
export class ListAcceptanceReportComponent implements OnInit {
  reports: any[] = [];
  filteredReports: any[] = [];
  searchKey: string = '';

  displayDetailDialog: boolean = false;
  detailReport: any;

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.reports = [
      { date: '2024-09-15', contractNumber: 'HD-2024-001', finalPrice: 244197273 },
      { date: '2024-01-09', contractNumber: 'HD-2024-002', finalPrice: 470977856 },
      { date: '2024-02-13', contractNumber: 'HD-2024-003', finalPrice: 837097080 },
      { date: '2024-11-11', contractNumber: 'HD-2024-004', finalPrice: 512661498 }
    ];
    this.filteredReports = [...this.reports];
  }

  onSearchChange() {
    this.filteredReports = this.reports.filter(report =>
      report.contractNumber.toLowerCase().includes(this.searchKey.toLowerCase())
    );
  }

  openAddDialog() {
    console.log('Mở form thêm mới');
  }

  openDetailDialog(report: any) {
    this.detailReport = report;
    this.displayDetailDialog = true;
  }

  openEditDialog(index: number) {
    console.log('Mở form chỉnh sửa:', this.filteredReports[index]);
  }

  deleteReport(index: number) {
    if (confirm('Bạn có chắc chắn muốn xóa biên bản này?')) {
      this.filteredReports.splice(index, 1);
    }
  }
}
