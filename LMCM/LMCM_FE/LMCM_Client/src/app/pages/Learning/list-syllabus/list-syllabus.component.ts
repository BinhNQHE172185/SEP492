import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { FormsModule } from '@angular/forms';
import { InputGroupModule } from 'primeng/inputgroup';
import { SyllabusApiService } from '../../../apis/syllabusAPIs/syllabus-api.service';
import { Subscription } from 'rxjs';
import { searchService } from '../../service/search/search-service.service';
import { ToastModule } from 'primeng/toast';
import { FileUploadModule } from 'primeng/fileupload';
import { DialogModule } from 'primeng/dialog';
import { MessageService } from 'primeng/api';

interface Syllabus {
  courseCode: string;
  courseName: string;
  courseNameEnglish: string;
  decisionNo: string;
  isActive: boolean;

}

interface PagingRequest {
  searchKey?: string;
  pageIndex: number;
  pageSize: number;
}

@Component({
  standalone: true,
  imports: [
    ToastModule, FileUploadModule, DialogModule, InputGroupModule, FormsModule, CommonModule, TableModule, ButtonModule, TagModule, CardModule, InputTextModule
  ],
  selector: 'app-list-syllabus',
  templateUrl: './list-syllabus.component.html',
  styleUrls: ['./list-syllabus.component.scss'],
  providers: [
    MessageService,
  ]
})
export class ListSyllabusComponent implements OnInit, OnDestroy {
  syllabuses: Syllabus[] = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  searchKey = '';

  displayImportDialog: boolean = false;
  uploadedFiles: any[] = [];

  private searchSubscription!: Subscription;

  constructor(private syllabusService: SyllabusApiService, private searchService: searchService, private messageService: MessageService) { }

  ngOnInit(): void {
    this.searchSubscription = this.searchService.searchQuery$.subscribe(
      (query) => {
        this.searchKey = query;
        this.loadSyllabuses();
      }
    );
  }

  loadSyllabuses(event?: any) {
    if (event) {
      this.pageNumber = Math.floor(event.first / event.rows) + 1;
      this.pageSize = event.rows;
    }

    const request: PagingRequest = {
      pageIndex: this.pageNumber,
      pageSize: this.pageSize,
      searchKey: this.searchKey,
    };

    this.syllabusService.getSyllabuses(request).subscribe(
      (response) => {

        if (!response.items || response.items.length === 0) {
          this.syllabuses = [];
          this.totalCount = 0;
          return;
        }

        this.syllabuses = response.items.map(item => ({
          courseCode: item.courseCode,
          courseName: item.courseName,
          courseNameEnglish: item.courseNameEnglish,
          decisionNo: item.decisionNo,
          isActive: item.isActive
        }));

        this.totalCount = response.totalCount;
      },
      (error) => {
        console.error("Lỗi khi tải danh sách syllabus:", error);
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

    this.syllabusService.importSyllabuses(formData).subscribe(
      () => {
        this.loadSyllabuses();
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

  deleteSyllabus(index: number) {
    this.syllabuses.splice(index, 1);
  }
}
