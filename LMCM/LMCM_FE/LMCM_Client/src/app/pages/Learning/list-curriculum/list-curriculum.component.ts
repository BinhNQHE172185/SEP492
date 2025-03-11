import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { FormsModule } from '@angular/forms';
import { InputGroupModule } from 'primeng/inputgroup';
import { Subscription } from 'rxjs';
import { CurriculumApiService } from '../../../apis/curriculumAPIs/curriculum-api.service';
import { searchService } from '../../service/search/search-service.service';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';

interface Curriculum {
  curriculumId: string;
  curriculumCode: string;
  name: string;
  description: string;
  decisionNo: string;
  approvedDate: string;
}

interface PagingRequest {
  searchKey?: string;
  pageIndex: number;
  pageSize: number;
}

@Component({
  standalone: true,
  imports: [
    InputGroupModule, FormsModule, CommonModule, TableModule, ButtonModule, CardModule,
    InputTextModule, ConfirmDialogModule, ToastModule
  ],
  selector: 'app-list-curriculum',
  templateUrl: './list-curriculum.component.html',
  styleUrls: ['./list-curriculum.component.scss'],
  providers: [ConfirmationService, MessageService]
})
export class ListCurriculumComponent implements OnInit, OnDestroy {
  curriculums: Curriculum[] = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  searchKey = '';

  private searchSubscription!: Subscription;

  constructor(
    private curriculumService: CurriculumApiService,
    private searchService: searchService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService
  ) {}

  ngOnInit(): void {
    this.searchSubscription = this.searchService.searchQuery$.subscribe(
      (query) => {
        this.searchKey = query;
        this.loadCurriculums();
      }
    );
    this.loadCurriculums();
  }

  loadCurriculums(event?: any) {
    if (event) {
      this.pageNumber = Math.floor(event.first / event.rows) + 1;
      this.pageSize = event.rows;
    }

    const request: PagingRequest = {
      pageIndex: this.pageNumber,
      pageSize: this.pageSize,
      searchKey: this.searchKey,
    };

    this.curriculumService.getCurriculums(request).subscribe(
      (response) => {
        if (!response.items || response.items.length === 0) {
          console.warn("Không có curriculum nào được trả về từ API.");
          this.curriculums = [];
          this.totalCount = 0;
          return;
        }

        this.curriculums = response.items;
        this.totalCount = response.totalCount;
      },
      (error) => {
        console.error("Lỗi khi tải danh sách curriculum:", error);
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

  confirmDelete(curriculumId: string, index: number) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa chương trình giảng dạy này không?',
      header: 'Xác nhận xóa',
      accept: () => {
        this.deleteCurriculum(curriculumId, index);
      }
    });
  }

  deleteCurriculum(curriculumId: string, index: number) {
    this.curriculumService.deleteCurriculum(curriculumId).subscribe(
      () => {
        this.curriculums.splice(index, 1);
        this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Xóa chương trình giảng dạy thành công!' });
      },
      (error) => {
        console.error("Lỗi khi xóa chương trình giảng dạy:", error);
        this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: error.error?.message || 'Xóa chương trình thất bại!' });
      }
    );
  }
}
