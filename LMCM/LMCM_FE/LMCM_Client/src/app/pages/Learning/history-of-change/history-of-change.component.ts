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
  imports: [
    ConfirmDialogModule, ToastModule, FileUploadModule, DialogModule, 
    InputGroupModule, FormsModule, CommonModule, TableModule, 
    ButtonModule, CardModule, InputTextModule, 

    // Thêm các module còn thiếu
    CalendarModule,      // Để chọn ngày (Ngày hoàn thành, Kỳ bắt đầu áp dụng)
    DropdownModule,      // Để hiển thị dropdown chọn "Loại thay đổi"
    InputTextModule  
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

  loadHistory(event?: any) {
    if (event) {
      console.log('📌 Sự kiện phân trang:', event);
      this.pageIndex = Math.floor(event.first / event.rows) + 1; // ✅ Tính pageIndex đúng
      this.pageSize = event.rows || this.pageSize;
    }
  
    const request = {
      searchKey: this.searchKey.trim(), 
      pageIndex: this.pageIndex,
      pageSize: this.pageSize
    };
  
    console.log('📤 Gửi request API:', request);
  
    this.learningMaterialService.getLearningMaterial(request).subscribe({
      next: (response) => {
        console.log('📥 Dữ liệu nhận được:', response);
        this.historyList = response.items;
        this.totalCount = response.totalCount;
      },
      error: (error) => {
        console.error('❌ Lỗi khi gọi API:', error);
        this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Không thể tải dữ liệu' });
      }
    });
  }
  
  
  paginate(event: any) {
    console.log(' Phân trang:', event);
    this.loadHistory(event);
  }

  editItem(item: any) {
    console.log('Chỉnh sửa:', item);
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
  displayDetailDialog = false;
selectedItem: any = {};
changeTypes = [
  { label: 'Xây mới', value: 'Xây mới' },
  { label: 'Chỉnh sửa', value: 'Chỉnh sửa' },
  { label: 'Xóa bỏ', value: 'Xóa bỏ' }
];

showDetail(item: any) {
  this.selectedItem = { ...item }; // Sao chép dữ liệu để chỉnh sửa không ảnh hưởng đến danh sách gốc
  this.displayDetailDialog = true;
}

saveDetail() {
  console.log('Lưu chi tiết:', this.selectedItem);
  this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Đã cập nhật dữ liệu' });
  this.displayDetailDialog = false;
}


  ngOnDestroy(): void {
    if (this.searchSubscription) {
      this.searchSubscription.unsubscribe();
    }
  }
}
