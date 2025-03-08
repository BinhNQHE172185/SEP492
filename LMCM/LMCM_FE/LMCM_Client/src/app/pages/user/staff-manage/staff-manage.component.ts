import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from "@angular/common";
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputGroupModule } from 'primeng/inputgroup';
import { FluidModule } from 'primeng/fluid';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { TagModule } from 'primeng/tag';
import { DialogModule } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';
import { UserApiService } from '../../../apis/userAPIs/user-api.service';
import { MessageService } from 'primeng/api';
import { FormsModule } from '@angular/forms';
import { ProfileStatus } from '../../../../shared/Constants/StatusConstants';
import { Subscription } from 'rxjs';
import { searchService } from '../../service/search/search-service.service';

interface PagingRequest {
  searchKey?: string;
  pageNumber: number;
  pageSize: number;
}

@Component({
  selector: 'app-staff-manage',
  imports: [
    CommonModule,
    CardModule,
    InputGroupModule,
    InputTextModule,
    ButtonModule,
    FluidModule,
    TableModule,
    IconFieldModule,
    InputIconModule,
    TagModule,
    DialogModule,
    ToastModule,
    FormsModule
  ],
  standalone: true,
  templateUrl: './staff-manage.component.html',
  styleUrls: ['./staff-manage.component.scss'],
  providers: [
    MessageService,
  ]
})
export class StaffManageComponent implements OnInit, OnDestroy {
  users: any[] = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  searchKey = '';
  userDialog: boolean = false;
  staffId: string = '';

  private searchSubscription!: Subscription;

  constructor(private apiService: UserApiService, private messageService: MessageService, private searchService: searchService) { }

  ngOnInit(): void {
    this.searchSubscription = this.searchService.searchQuery$.subscribe(
      (query) => {
        this.searchKey = query;
        this.loadUser();
      }
    );
  }

  loadUser() {
    const request: PagingRequest = {
      searchKey: this.searchKey,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize
    };
    this.apiService.getListUser(request).subscribe(
      (response) => {
        console.log(response)
        this.users = response.items;
      },
      (error) => {
        this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: error.error.message });
      }
    );
  }

  cols = [
    { field: 'name', header: 'Tên' },
    { field: 'status', header: 'Trạng thái' },
    { field: 'email', header: 'Email' },
    { field: 'actions', header: 'Hành động' }
  ];

  getStatusLabel(status: number): string {
    return ProfileStatus[status] || "Không xác định";
  }

  getTagSeverity(status: string | number): "success" | "info" | "danger" | "warn" | "secondary" | "contrast" | undefined {
    const statusNumber = +status; // Ép kiểu từ string sang number
    switch (statusNumber) {
      case 1:
        return "info";     // Đang chờ
      case 2:
        return "success";  // Hoạt động
      case 3:
        return "danger";   // Ngừng hoạt động
      default:
        return "secondary"; // Mặc định nếu giá trị không hợp lệ
    }
  }

  openNew() {
    this.userDialog = true;
  }
  hideDialog() {
    this.userDialog = false;
  }
  saveUser() {
    if (!this.staffId.trim()) {
      return; // Không gửi nếu email trống
    }

    this.apiService.createAccount(this.staffId).subscribe(
      (response) => {
        this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Nhân viên đã được thêm' });
        this.hideDialog(); // Ẩn dialog sau khi thêm thành công
        this.loadUser();
      },
      (error) => {
        this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: error.error.message });
      }
    );
  }

  ngOnDestroy(): void {
    if (this.searchSubscription) {
      this.searchSubscription.unsubscribe();
    }
  }

  onSearchChange(query: string) {
    this.searchService.updateSearchQuery(query);
  }
}
