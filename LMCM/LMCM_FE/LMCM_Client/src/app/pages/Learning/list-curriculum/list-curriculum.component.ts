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
import { RouterLink } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api'; // Thêm import này
import { ConfirmDialog } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { FileUploadModule } from 'primeng/fileupload';
import { DialogModule } from 'primeng/dialog';
interface Curriculum {
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
    ToastModule, FileUploadModule, DialogModule, InputGroupModule, FormsModule, CommonModule, TableModule, ButtonModule, CardModule, InputTextModule, ConfirmDialog
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

  private searchSubscription!: Subscription;

  constructor(private curriculumService: CurriculumApiService, private searchService: searchService, private messageService: MessageService) { }

  ngOnInit(): void {
    this.searchSubscription = this.searchService.searchQuery$.subscribe(
      (query) => {
        this.searchKey = query;
        this.loadCurriculums();
      }
    );
  }


  deleteCurriculum(code: string) {
    this.curriculums = this.curriculums.filter(item => item.curriculumCode !== code);
    this.totalCount = this.curriculums.length; // Cập nhật tổng số bản ghi
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

    this.curriculumService.importCurriculums(formData).subscribe(
      () => {
        this.loadCurriculums();
        this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Nhập dữ liệu thành công' });
      },
      (error) => {
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
}
