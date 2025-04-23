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
import { Router, RouterLink } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api'; // Thêm import này
import { ConfirmDialog, ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { FileUploadModule } from 'primeng/fileupload';
import { DialogModule } from 'primeng/dialog';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

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
    ProgressSpinnerModule, ConfirmDialogModule, ToastModule, FileUploadModule, DialogModule, InputGroupModule, FormsModule, CommonModule, TableModule, ButtonModule, CardModule, InputTextModule, ConfirmDialog
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

  displayImportDialog: boolean = false;
  uploadedFiles: any[] = [];

  isLoading: boolean = false;

  private searchSubscription!: Subscription;

  constructor(private curriculumService: CurriculumApiService,
    private searchService: searchService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.searchSubscription = this.searchService.searchQuery$.subscribe(
      (query) => {
        this.searchKey = query;
        this.loadCurriculums();
      }
    );
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
          this.curriculums = [];
          this.totalCount = 0;
          return;
        }

        this.curriculums = response.items.map(item => ({
          curriculumId: item.curriculumId,
          curriculumCode: item.curriculumCode,
          name: item.name,
          description: item.description,
          decisionNo: item.decisionNo,
          approvedDate: item.approvedDate
        }));

        this.totalCount = response.totalCount;
      },
      (error) => {
        console.error("Lỗi khi tải danh sách curriculum:", error);
      }
    );
  }

  goToDetail(curriculumId: string) {
    this.router.navigate([`/learning/curriculum/${curriculumId}`]);
  }

  showImportDialog() {
    this.displayImportDialog = true;
  }

  closeDialog() {
    this.displayImportDialog = false;
  }

  onUpload(event: any) {
    const file = event.files[0];
    const formData = new FormData();
    formData.append('file', file);
    this.isLoading = true;
    this.curriculumService.importCurriculums(formData).subscribe(
      () => {
        this.loadCurriculums();
        this.isLoading = false;
        this.closeDialog();
        this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Nhập dữ liệu thành công' });
      },
      (error) => {
        this.isLoading = false;
        this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: error.error.message });
      }
    );
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

  deleteCurriculumn(id: any) {
    this.confirmationService.confirm({
      header: 'Xóa dữ liệu',
      message: 'Bạn có chắc chắn muốn xóa? Hành động này là không thể hoàn tác.',
      acceptLabel: 'Đồng ý',
      rejectLabel: 'Hủy',
      accept: () => {
        this.isLoading = true;
        this.curriculumService.deleteCurriculums(id).subscribe(
          (response) => {
            this.loadCurriculums();
            this.isLoading = false;
            this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Xóa dữ liệu thành công' });
          },
          (error) => {
            this.isLoading = false;
            this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: error.error.message });
          }
        );
      }
    });
  }
}
