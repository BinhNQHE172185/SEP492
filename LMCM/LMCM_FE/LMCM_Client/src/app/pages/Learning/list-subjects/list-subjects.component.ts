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
    InputGroupModule, FormsModule, CommonModule, TableModule, ButtonModule, TagModule, CardModule, InputTextModule
  ],
  selector: 'app-list-subjects',
  templateUrl: './list-subjects.component.html',
  styleUrls: ['./list-subjects.component.scss'],
})
export class ListSubjectsComponent implements OnInit, OnDestroy {
  subjects: Subject[] = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  searchKey = '';

  private searchSubscription!: Subscription;

  constructor(private subjectService: SubjectApiService, private searchService: searchService) { }

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

  ngOnDestroy(): void {
    if (this.searchSubscription) {
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
