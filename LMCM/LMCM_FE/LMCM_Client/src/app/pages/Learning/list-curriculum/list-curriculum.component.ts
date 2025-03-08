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
import { ConfirmationService } from 'primeng/api'; // Thêm import này
import { ConfirmDialog } from 'primeng/confirmdialog';
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
    InputGroupModule, FormsModule, CommonModule, TableModule, ButtonModule, CardModule, InputTextModule, ConfirmDialog
  ],
  selector: 'app-list-curriculum',
  templateUrl: './list-curriculum.component.html',
  styleUrls: ['./list-curriculum.component.scss'],
  providers: [ConfirmationService] // ✅ Thêm vào providers
})
export class ListCurriculumComponent implements OnInit, OnDestroy {
  curriculums: Curriculum[] = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  searchKey = '';

  private searchSubscription!: Subscription;

  constructor(private curriculumService: CurriculumApiService, private searchService: searchService) { }

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
        console.log("Dữ liệu nhận được:", response);

        if (!response.items || response.items.length === 0) {
          console.warn("Không có curriculum nào được trả về từ API.");
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

  ngOnDestroy(): void {
    if (this.searchSubscription) {
      this.searchSubscription.unsubscribe();
    }
  }

  onSearchChange(query: string) {
    this.searchService.updateSearchQuery(query);
  }
}
