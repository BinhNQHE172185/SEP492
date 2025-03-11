import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { FormsModule } from '@angular/forms';
import { InputGroupModule } from 'primeng/inputgroup';
import { SubjectApiService } from '../../../apis/subjectAPIs/subject-api.service';
import { Subscription } from 'rxjs';
import { searchService } from '../../service/search/search-service.service';
import { MessageService } from 'primeng/api';
import { DialogModule } from 'primeng/dialog';
import { FileUploadModule } from 'primeng/fileupload';
import { ToastModule } from 'primeng/toast';

interface Subject {
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
  searchKey?: string;
  pageIndex: number;
  pageSize: number;
}

@Component({
  standalone: true,
  imports: [
    ToastModule, FileUploadModule, DialogModule, InputGroupModule, FormsModule, CommonModule, TableModule, ButtonModule, TagModule, CardModule, InputTextModule
  ],
  selector: 'app-list-subjects',
  templateUrl: './list-subjects.component.html',
  styleUrls: ['./list-subjects.component.scss'],
  providers: [
    MessageService,
  ]
})
export class ListSubjectsComponent implements OnInit, OnDestroy {
  subjects: Subject[] = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  searchKey = '';

  displayImportDialog: boolean = false;
  uploadedFiles: any[] = [];

  private searchSubscription!: Subscription;

  constructor(private subjectService: SubjectApiService, private searchService: searchService, private messageService: MessageService) { }

  ngOnInit(): void {
    this.searchSubscription = this.searchService.searchQuery$.subscribe(
      (query) => {
        this.searchKey = query;
        this.loadSubjects();
      }
    );
  }

  loadSubjects(event?: any) {
    if (event) {
      this.pageNumber = Math.floor(event.first / event.rows) + 1;
      this.pageSize = event.rows;
    }

    const request: PagingRequest = {
      pageIndex: this.pageNumber,
      pageSize: this.pageSize,
      searchKey: this.searchKey,
    };
    this.subjectService.getSubjects(request).subscribe(
      (response) => {
        this.subjects = response.items;
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
    const file = event.files[0];
    const formData = new FormData();
    formData.append('file', file);

    this.subjectService.importSubjects(formData).subscribe(
      () => {
        this.loadSubjects(); // Load lại dữ liệu sau khi import
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

  getTagValue(value: boolean): 'Yes' | 'No' {
    return value ? 'Yes' : 'No';
  }

  deleteSubject(index: number) {
    this.subjects.splice(index, 1);
  }
}
