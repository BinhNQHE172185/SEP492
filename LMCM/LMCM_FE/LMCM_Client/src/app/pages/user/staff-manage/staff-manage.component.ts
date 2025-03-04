import { Component } from '@angular/core';
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

interface Column {
  field: string;
  header: string;
  customExportHeader?: string;
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
    DialogModule
  ],
  standalone: true,
  templateUrl: './staff-manage.component.html',
  styleUrls: ['./staff-manage.component.scss'],
})
export class StaffManageComponent {
  userDialog: boolean = false;
  submitted: boolean = false;

  cols = [
    { field: 'name', header: 'Tên' },
    { field: 'status', header: 'Trạng thái' },
    { field: 'email', header: 'Email' },
    { field: 'actions', header: 'Hành động' }
  ];

  staffList = [
    { id: 1, name: 'Nguyễn Văn A', status: 'Hoạt động', email: 'a@example.com' },
    { id: 2, name: 'Trần Thị B', status: 'Tạm nghỉ', email: 'b@example.com' },
    { id: 3, name: 'Lê Văn C', status: 'Hoạt động', email: 'c@example.com' },
    { id: 4, name: 'Phạm Thị D', status: 'Đã nghỉ', email: 'd@example.com' }
  ];
  getTagSeverity(status: string): 'success' | 'warn' | 'danger' | 'info' {
    switch (status) {
      case 'Hoạt động':
        return 'success'; // Màu xanh lá
      case 'Tạm nghỉ':
        return 'warn'; // Màu vàng (dùng 'warn' thay vì 'warning')
      case 'Đã nghỉ':
        return 'danger'; // Màu đỏ
      default:
        return 'info'; // Màu mặc định
    }
  }
  openNew() {
    this.submitted = false;
    this.userDialog = true;
    console.log("leh bucu")
  }
  hideDialog() {
    this.userDialog = false;
    this.submitted = false;
  }
}
