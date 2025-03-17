import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { FormsModule } from '@angular/forms';
import { InputGroupModule } from 'primeng/inputgroup';
import { Subscription } from 'rxjs';
import { LearningMaterialApiService } from '../../../apis/learning-materialAPIs/learning-material-api.service';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { FileUploadModule } from 'primeng/fileupload';
import { DialogModule } from 'primeng/dialog';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';

@Component({
    standalone: true,
    imports: [ConfirmDialogModule, ToastModule, FileUploadModule, DialogModule, InputGroupModule, FormsModule, CommonModule, TableModule, ButtonModule, CardModule, InputTextModule, CalendarModule, DropdownModule, InputTextModule],
    selector: 'app-history-of-change',
    templateUrl: './history-of-change.component.html',
    styleUrls: ['./history-of-change.component.scss'],
    providers: [ConfirmationService, MessageService]
})
export class HistoryOfChangeComponent implements OnInit, OnDestroy {
    historyList: any[] = [];
    totalCount = 0;
    pageSize = 10;
    pageIndex = 1;
    searchKey = '';
    private searchSubscription!: Subscription;
    displayDetailDialog = false;
    displayAddDialog = false;
    selectedItem: any = {};
    newHistory: any = {};
    displayEditDialog: boolean = false;

    editHistory: any = {};
    constructor(
        private learningMaterialService: LearningMaterialApiService,
        private confirmationService: ConfirmationService,
        private messageService: MessageService
    ) {}

    ngOnInit(): void {
        this.loadHistory();
    }

    getUserIdFromLocalStorage(): string | null {
        return localStorage.getItem('userId');
    }

    loadHistory(event?: any) {
        if (event) {
            this.pageIndex = Math.floor(event.first / event.rows) + 1;
            this.pageSize = event.rows || this.pageSize;
        }

        const request = {
            searchKey: this.searchKey.trim(),
            pageIndex: this.pageIndex,
            pageSize: this.pageSize
        };

        this.learningMaterialService.getLearningMaterial(request).subscribe({
            next: (response) => {
                console.log('Dữ liệu nhận được:', response);
                this.historyList = response.items;
                this.totalCount = response.totalCount;
            },
            error: (error) => {
                console.error('Lỗi khi gọi API:', error);
                this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Không thể tải dữ liệu' });
            }
        });
    }

    paginate(event: any) {
        console.log(' Phân trang:', event);
        this.loadHistory(event);
    }
    showDetail(item: any) {
        if (!item) {
            return;
        }

        this.selectedItem = {
            ...item,
            completionDate: item.completionDate ? new Date(item.completionDate) : null,
            startTerm: item.startTerm ? new Date(item.startTerm) : null
        };

        this.displayDetailDialog = true;
    }
    closeDetailDialog() {
        this.displayDetailDialog = false;
    }

    editDetail(item: any) {
        console.log("Editing item:", item);
        this.editHistory = {
            ...item,
            completionDate: item.completionDate ? new Date(item.completionDate) : null,
            startTerm: item.startTerm ? new Date(item.startTerm + "-01") : null, 
        };
        this.displayEditDialog = true;
    }
    
    updateHistory() {
        console.log('Updated history:', this.editHistory);
        this.displayEditDialog = false;
    }

    confirmDelete(item: any) {
        this.confirmationService.confirm({
            message: 'Bạn có chắc chắn muốn xóa bản ghi này?',
            header: 'Xác nhận',
            accept: () => {
                this.deleteItem(item);
            }
        });
    }

    deleteItem(item: any) {
        console.log('Xóa:', item);
        this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Đã xóa bản ghi' });
    }

    openAddDialog() {
        this.newHistory = {};
        this.displayAddDialog = true;
    }

    addHistory() {
        const userId = this.getUserIdFromLocalStorage();
        if (!userId) {
            this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Không tìm thấy thông tin người dùng' });
            return;
        }
        if (!this.newHistory.changeType || !this.newHistory.learningMaterialType) {
            this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Vui lòng nhập đầy đủ thông tin' });
            return;
        }

        const request = {
            userId: userId,
            contractId: this.newHistory.contractNumber || null,
            newMaterialId: this.newHistory.newMaterialId || null,
            oldMaterialId: this.newHistory.oldMaterialId || null,
            learningMaterialType: this.newHistory.learningMaterialType,
            changeType: this.newHistory.changeType,
            changeDescription: this.newHistory.changeDescription || '',
            completionDate: this.newHistory.completionDate || new Date().toISOString(),
            startTerm: this.newHistory.startTerm || '',
            courseCode: this.newHistory.courseCode || ''
        };

        console.log('Gửi request thêm lịch sử:', request);

        this.learningMaterialService.createLearningMaterialHistory(request).subscribe({
            next: () => {
                this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Đã thêm lịch sử thay đổi' });
                this.displayAddDialog = false;
                this.loadHistory();
            },
            error: (error) => {
                console.error('Lỗi khi thêm lịch sử thay đổi:', error);
                this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Không thể thêm lịch sử thay đổi' });
            }
        });
    }

    ngOnDestroy(): void {
        if (this.searchSubscription) {
            this.searchSubscription.unsubscribe();
        }
    }
}
