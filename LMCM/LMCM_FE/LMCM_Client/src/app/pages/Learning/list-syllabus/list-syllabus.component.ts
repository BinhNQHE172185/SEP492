import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { FormsModule } from '@angular/forms';
import { InputGroupModule } from 'primeng/inputgroup';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { SyllabusApiService } from '../../../apis/syllabusAPIs/syllabus-api.service';
import { Subscription } from 'rxjs';
import { searchService } from '../../service/search/search-service.service';

interface Syllabus {
  syllabusId: string;
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
    InputGroupModule, FormsModule, CommonModule, TableModule, ButtonModule, TagModule, 
    CardModule, InputTextModule, ConfirmDialogModule
  ],
  providers: [ConfirmationService],
  selector: 'app-list-syllabus',
  templateUrl: './list-syllabus.component.html',
  styleUrls: ['./list-syllabus.component.scss'],
})
export class ListSyllabusComponent implements OnInit, OnDestroy {
  syllabuses: Syllabus[] = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  searchKey = '';

  private searchSubscription!: Subscription;

  constructor(
    private syllabusService: SyllabusApiService,
    private searchService: searchService,
    private confirmationService: ConfirmationService
  ) {}

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
        console.log("Dữ liệu nhận được:", response);

        if (!response.items || response.items.length === 0) {
          console.warn("Không có syllabus nào được trả về từ API.");
          this.syllabuses = [];
          this.totalCount = 0;
          return;
        }

        this.syllabuses = response.items.map(item => ({
          syllabusId: item.syllabusId,
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

  ngOnDestroy(): void {
    if (this.searchSubscription) {
      this.searchSubscription.unsubscribe();
    }
  }

  onSearchChange(query: string) {
    this.searchService.updateSearchQuery(query);
  }

  confirmDelete(syllabusId: string) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa syllabus này không?',
      header: 'Xác nhận',
      accept: () => {
        this.deleteSyllabus(syllabusId);
      }
    });
    console.log("Sending delete request with ID:", syllabusId);

  }
  
  deleteSyllabus(syllabusId: string) {
    this.syllabusService.deleteSyllabus(syllabusId).subscribe(
      () => {
        this.syllabuses = this.syllabuses.filter(syllabus => syllabus.syllabusId !== syllabusId);
        this.totalCount--; // Cập nhật tổng số bản ghi
      },
      (error) => {
        console.error("Lỗi khi xóa syllabus:", error);
      }
    );
  }
  
  
  
}
