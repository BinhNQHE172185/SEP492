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
import { MessageService } from 'primeng/api';

import { ConfirmationService } from 'primeng/api';
@Component({
    selector: 'app-list-acceptance-report',
    standalone: true,
    imports: [
        CommonModule, TableModule, ButtonModule, InputTextModule, InputGroupModule, 
        CardModule, ConfirmDialogModule, ToastModule, FileUploadModule, DialogModule, 
        FormsModule, TagModule, TextareaModule, CalendarModule, ConfirmDialogModule, 
    ],
    templateUrl: './list-acceptance-report.component.html',
    styleUrls: ['./list-acceptance-report.component.scss'],
    providers: [ConfirmationService, MessageService]
})
export class ListAcceptanceReportComponent implements OnInit {
    reports: any[] = [];
    filteredReports: any[] = [];
    searchKey: string = '';
    
    displayDetailDialog: boolean = false;
    displayEditDialog: boolean = false;
    detailReport: any;
    selectedItem: any = {};

    constructor(private messageService: MessageService, private confirmationService: ConfirmationService) {}
    

    ngOnInit() {
        this.loadData();
    }
    
    loadData() {
        this.reports = [
            {
                date: '2024-09-15',
                contractNumber: 'HD-2024-001',
                finalPrice: 244197273,
                observer: 'Quyen Le Ngoc',
                content: 'Nghiệm thu hệ thống quản lý doanh nghiệp...'
            },
            {
                date: '2024-01-09',
                contractNumber: 'HD-2024-002',
                finalPrice: 470977856,
                observer: 'Quyen Le Ngoc',
                content: 'Nghiệm thu hệ thống quản lý doanh nghiệp...'
            },
            {
                date: '2024-02-13',
                contractNumber: 'HD-2024-003',
                finalPrice: 837097080,
                observer: 'Quyen Le Ngoc',
                content: 'Nghiệm thu hệ thống quản lý doanh nghiệp...'
            },
            {
                date: '2024-11-11',
                contractNumber: 'HD-2024-004',
                finalPrice: 512661498,
                observer: 'Quyen Le Ngoc',
                content: 'Nghiệm thu hệ thống quản lý doanh nghiệp...'
            }
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

    openEditDialog(report: any) {
        this.selectedItem = { ...report }; 
        this.displayEditDialog = true;
      
    }
    confirmDelete(report: any, index: number) {
        this.confirmationService.confirm({
            message: 'Bạn có chắc chắn muốn xóa biên bản này?',
            header: 'Xác nhận xóa',
         
            accept: () => {
                this.deleteReport(index);
            }
        });
    }
    

    deleteReport(index: number) {
        this.filteredReports.splice(index, 1);
        this.reports = [...this.filteredReports]; 
        this.messageService.add({ severity: 'warn', summary: 'Đã xóa', detail: 'Biên bản đã bị xóa' });
    }
    

    saveEdit() {
        const index = this.reports.findIndex(report => report.contractNumber === this.selectedItem.contractNumber);
        if (index !== -1) {
            this.reports[index] = { ...this.selectedItem };
            this.filteredReports = [...this.reports]; 
            this.displayEditDialog = false;
            this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Cập nhật biên bản thành công!' });
        }
    }


    closeDialog(dialogType: string) {
        if (dialogType === 'detail') {
            this.displayDetailDialog = false;
        } else if (dialogType === 'edit') {
            this.displayEditDialog = false;
        }
    }
}
