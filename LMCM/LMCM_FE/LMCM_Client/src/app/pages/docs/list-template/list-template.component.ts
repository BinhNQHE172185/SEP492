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
import { MessageService, ConfirmationService } from 'primeng/api';

@Component({
    selector: 'app-list-template',
    standalone: true,
    imports: [
        CommonModule, TableModule, ButtonModule, InputTextModule, InputGroupModule, 
        CardModule, ConfirmDialogModule, ToastModule, FileUploadModule, DialogModule, 
        FormsModule, TagModule, TextareaModule
    ],
    templateUrl: './list-template.component.html',
    styleUrls: ['./list-template.component.scss'],
    providers: [ConfirmationService, MessageService]
})
export class ListTemplateComponent implements OnInit {
    documents: any[] = [];
    filteredDocuments: any[] = [];
    searchKey: string = '';
    displayAddDialog: boolean = false;
    displayEditDialog: boolean = false;
    selectedDocument: any = {};
    newDocument: any = {};

    constructor(private messageService: MessageService, private confirmationService: ConfirmationService) {}

    ngOnInit() {
        this.loadData();
    }
    
    loadData() {
        this.documents = [
            { name: 'Mẫu tờ trình số 01/2024', type: 'Tờ trình', status: 'Đang sử dụng' },
            { name: 'Mẫu hợp đồng xây dựng học liệu', type: 'Hợp đồng', status: 'Đang sử dụng' },
            { name: 'Biên bản nghiệm thu học liệu', type: 'Biên bản', status: 'Tạm ngưng' },
            { name: 'Quyết định định mức xây dựng học liệu', type: 'Quyết định', status: 'Đang sử dụng' }
        ];
        this.filteredDocuments = [...this.documents];
    }

    onSearchChange() {
        this.filteredDocuments = this.documents.filter(doc =>
            doc.name.toLowerCase().includes(this.searchKey.toLowerCase())
        );
    }

    openAddDialog() {
        this.newDocument = { name: '', type: '', status: '' };
        this.displayAddDialog = true;
    }

    saveNewDocument() {
        if (!this.newDocument.name || !this.newDocument.type || !this.newDocument.status) {
            this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Vui lòng điền đầy đủ thông tin!' });
            return;
        }

        this.documents.push({ ...this.newDocument });
        this.filteredDocuments = [...this.documents];
        this.displayAddDialog = false;

        this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Thêm tài liệu thành công!' });
    }

    openEditDialog(document: any) {
        this.selectedDocument = { ...document }; 
        this.displayEditDialog = true;
    }

    confirmDelete(document: any, index: number) {
        this.confirmationService.confirm({
            message: 'Bạn có chắc chắn muốn xóa tài liệu này?',
            header: 'Xác nhận xóa',
            accept: () => {
                this.deleteDocument(index);
            }
        });
    }

    deleteDocument(index: number) {
        this.filteredDocuments.splice(index, 1);
        this.documents = [...this.filteredDocuments]; 
        this.messageService.add({ severity: 'warn', summary: 'Đã xóa', detail: 'Tài liệu đã bị xóa' });
    }

    saveEdit() {
        const index = this.documents.findIndex(doc => doc.name === this.selectedDocument.name);
        if (index !== -1) {
            this.documents[index] = { ...this.selectedDocument };
            this.filteredDocuments = [...this.documents]; 
            this.displayEditDialog = false;
            this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Cập nhật tài liệu thành công!' });
        }
    }

    closeDialog(dialogType: string) {
        if (dialogType === 'edit') {
            this.displayEditDialog = false;
        } else if (dialogType === 'add') {
            this.displayAddDialog = false;
        }
    }
}
