import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { searchService } from '../../service/search/search-service.service';
import { ConfirmationService, MessageService } from 'primeng/api';
import { InputTextModule } from 'primeng/inputtext';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InputGroupModule } from 'primeng/inputgroup';
import { DialogModule } from 'primeng/dialog';
import { FileUploadModule } from 'primeng/fileupload';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { SyllabusApiService } from '../../../apis/syllabusAPIs/syllabus-api.service';
import { ActivatedRoute } from '@angular/router';

interface Subject {
  subjectId: string;
  subjectCode: string;
  subjectName: string;
  englishName: string;
  previousCode?: string;
  isConstructivist: boolean;
  method: string;
  duration: string;
  reality: string;
}

interface PagingRequest {
  id: string;
  searchKey?: string;
  pageIndex: number;
  pageSize: number;
}

@Component({
  selector: 'app-syllabus-history',
  standalone: true,
  imports: [
    TagModule, ConfirmDialogModule, ToastModule, FileUploadModule, DialogModule, InputGroupModule, FormsModule, CommonModule, TableModule, ButtonModule, TagModule, CardModule, InputTextModule
  ],
  templateUrl: './syllabus-history.component.html',
  styleUrl: './syllabus-history.component.scss',
  providers: [
    MessageService,
    ConfirmationService
  ]
})
export class SyllabusHistoryComponent implements OnInit, OnDestroy {
  subjectId: string = '';
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  searchKey = '';
  historyList: any[] = [];

  displayImportDialog: boolean = false;
  uploadedFiles: any[] = [];

  private searchSubscription!: Subscription;

  constructor(private syllabusService: SyllabusApiService,
    private searchService: searchService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private route: ActivatedRoute,
  ) { }

  ngOnInit(): void {
    this.subjectId = this.route.snapshot.paramMap.get('id') || '';
    this.searchSubscription = this.searchService.searchQuery$.subscribe(
      (query) => {
        this.searchKey = query;
        this.loadSyllabus();
      }
    );
  }

  loadSyllabus(event?: any) {
    if (event) {
      this.pageNumber = Math.floor(event.first / event.rows) + 1;
      this.pageSize = event.rows;
    }

    const request: PagingRequest = {
      id: this.subjectId,
      pageIndex: this.pageNumber,
      pageSize: this.pageSize,
      searchKey: this.searchKey,
    };
    this.syllabusService.getSyllabusHistory(request).subscribe(
      (response) => {
        this.historyList = response.items;
        this.totalCount = response.totalCount;
      },
      (error) => {
        console.error("Lỗi khi tải danh sách môn học:", error);
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
    // const file = event.files[0];
    // const formData = new FormData();
    // formData.append('file', file);

    // this.subjectService.importSubjects(formData).subscribe(
    //   () => {
    //     this.loadSubjects(); // Load lại dữ liệu sau khi import
    //     this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Nhập dữ liệu thành công' });
    //   },
    //   (error) => {
    //     this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: error.error.message });
    //   }
    // );
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

  getTagValue(value: boolean): 'Yes' | 'No' {
    return value ? 'Yes' : 'No';
  }

  deleteSubject(id: any) {
    // this.confirmationService.confirm({
    //   header: 'Xóa dữ liệu',
    //   message: 'Bạn có chắc chắn muốn xóa? Hành động này là không thể hoàn tác.',
    //   acceptLabel: 'Đồng ý',
    //   rejectLabel: 'Hủy',
    //   accept: () => {
    //     this.subjectService.deleteSubjects(id).subscribe(
    //       (response) => {
    //         this.loadSubjects();
    //         this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Xóa dữ liệu thành công' });
    //       },
    //       (error) => {
    //         this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: error.error.message });
    //       }
    //     );
    //   }
    // });
  }
}
