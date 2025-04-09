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
import { MessageService } from 'primeng/api';
import { FormsModule } from '@angular/forms';
import { ProfileStatus } from '../../../../shared/Constants/StatusConstants';
import { Subscription } from 'rxjs';
import { searchService } from '../../service/search/search-service.service';
import { UserApiService } from '../../../apis/userAPIs/user-api.service';
import { DropdownModule } from 'primeng/dropdown';
import { UserRole } from '../../../../shared/Constants/UserConstants';

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
    FormsModule,
    DropdownModule
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

  cols = [
    { field: 'name', header: 'Tên' },
    { field: 'status', header: 'Trạng thái' },
    { field: 'email', header: 'Email' },
    { field: 'actions', header: 'Hành động' }
  ];

  roles = [
    { label: 'Trưởng phòng', value: 'Head of Department' },
    { label: 'Nhân viên', value: 'Staff' }
  ];

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

  getStatusLabel(status: number): string {
    return ProfileStatus[status] || "Không xác định";
  }

  getRoleLabel(role: string): string {
    return UserRole[role as keyof typeof UserRole] || "Không xác định";
  }

  getTagSeverity(status: string | number): "success" | "info" | "danger" | "warn" | "secondary" | "contrast" | undefined {
    const statusNumber = +status;
    switch (statusNumber) {
      case 1:
        return "info";
      case 2:
        return "success";
      case 3:
        return "danger";
      default:
        return "secondary";
    }
  }

  getTagSeverityRole(status: string): "success" | "info" | "danger" | "warn" | "secondary" | "contrast" | undefined {
    switch (status) {
      case "Staff":
        return "info";
      case "Head of Department":
        return "success";
      default:
        return "secondary";
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

  onRoleChange(userId: string, newRole: string) {
    this.apiService.changeRole(userId, newRole).subscribe({
      next: (response) => {
        this.messageService.add({ severity: 'success', summary: 'Thành công', detail: response.message });
        this.loadUser();
      },
      error: (error) => {
        this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: error.error.message });
      }
    });
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
}
