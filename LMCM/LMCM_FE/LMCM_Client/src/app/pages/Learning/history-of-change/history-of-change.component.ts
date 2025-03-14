import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { FormsModule } from '@angular/forms';
import { InputGroupModule } from 'primeng/inputgroup';
import { Subscription } from 'rxjs';
import {LearningMaterialApiService } from '../../../apis/learning-materialAPIs/learning-material-api.service'
import { searchService } from '../../service/search/search-service.service';
import { RouterLink } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api'; // Thêm import này
import { ConfirmDialog, ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { FileUploadModule } from 'primeng/fileupload';
import { DialogModule } from 'primeng/dialog';
@Component({
  standalone: true,
  imports: [
    ConfirmDialogModule,ToastModule, FileUploadModule, DialogModule, InputGroupModule, FormsModule, CommonModule, TableModule, ButtonModule, CardModule, InputTextModule, ConfirmDialog
  ],
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

  constructor(
    private learningMaterialService: LearningMaterialApiService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService
  ) {}

  ngOnInit(): void {
    this.loadHistory();
  }

  loadHistory() {
    const request = {
      searchKey: this.searchKey,
      pageIndex: this.pageIndex,
      pageSize: this.pageSize
    };

    this.learningMaterialService.getLearningMaterial(request).subscribe({
      next: (response) => {
        this.historyList = response.items;
        this.totalCount = response.totalCount;
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Không thể tải dữ liệu' });
      }
    });
  }

  paginate(event: any) {
    this.pageIndex = event.page + 1;
    this.loadHistory();
  }

  editItem(item: any) {
    console.log('Edit:', item);
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
    console.log('Delete:', item);
    this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Đã xóa bản ghi' });
  }

  ngOnDestroy(): void {
    if (this.searchSubscription) {
      this.searchSubscription.unsubscribe();
    }
  }
}
