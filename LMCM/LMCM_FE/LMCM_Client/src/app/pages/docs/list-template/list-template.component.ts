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
import { MessageService, ConfirmationService } from 'primeng/api';
import { DropdownModule } from 'primeng/dropdown';
@Component({
    selector: 'app-list-template',
    standalone: true,
    imports: [
        CommonModule, TableModule, ButtonModule, InputTextModule, InputGroupModule, 
        CardModule, ConfirmDialogModule, ToastModule, DialogModule, FormsModule, TagModule, DropdownModule
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
    displayDetailDialog: boolean = false;
    selectedDocument: any = {};
    newDocument: any = {};

    constructor(private messageService: MessageService, private confirmationService: ConfirmationService) {}

    ngOnInit() {
        this.loadData();
    }
    documentTypes = [
        { label: 'Tờ trình', value: 'Tờ trình' },
        { label: 'Hợp đồng', value: 'Hợp đồng' },
        { label: 'Biên bản', value: 'Biên bản' },
        { label: 'Quyết định', value: 'Quyết định' }
    ];
    
    documentStatuses = [
        { label: 'Đang sử dụng', value: 'Đang sử dụng' },
        { label: 'Tạm ngưng', value: 'Tạm ngưng' }
    ];
    loadData() {
        this.documents = [
            { 
                name: 'Mẫu tờ trình số 01/2024', 
                type: 'Tờ trình', 
                status: 'Đang sử dụng', 
                createdBy: 'Nguyễn Văn A',
                createdAt: '2024-01-10',
                lastModified: '2024-03-20',
                description: 'Chi tiết về mẫu tờ trình số 01/2024' 
            },
            { 
                name: 'Mẫu hợp đồng xây dựng học liệu', 
                type: 'Hợp đồng', 
                status: 'Đang sử dụng',
                createdBy: 'Trần Thị B',
                createdAt: '2024-02-05',
                lastModified: '2024-03-18',
                description: 'Chi tiết về hợp đồng xây dựng học liệu' 
            },
            { 
                name: 'Biên bản nghiệm thu học liệu', 
                type: 'Biên bản', 
                status: 'Tạm ngưng',
                createdBy: 'Lê Minh C',
                createdAt: '2024-02-15',
                lastModified: '2024-03-19',
                description: 'Chi tiết về biên bản nghiệm thu học liệu' 
            },
            { 
                name: 'Quyết định định mức xây dựng học liệu', 
                type: 'Quyết định', 
                status: 'Đang sử dụng',
                createdBy: 'Phạm Duy D',
                createdAt: '2024-03-01',
                lastModified: '2024-03-21',
                description: 'Chi tiết về quyết định định mức' 
            }
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

    openDetailDialog(document: any) {
        this.selectedDocument = { ...document }; 
        this.displayDetailDialog = true;
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
        } else if (dialogType === 'detail') {
            this.displayDetailDialog = false;
        }
    }
}
