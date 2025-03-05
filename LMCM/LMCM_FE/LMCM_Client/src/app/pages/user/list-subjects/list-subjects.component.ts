import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { FormsModule } from '@angular/forms';
import { InputGroupModule } from 'primeng/inputgroup';
import { SubjectApiService } from '../../../apis/subjectAPIs/subject-api.service';

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
  pageNumber: number;
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
export class ListSubjectsComponent implements OnInit {
  
  subjects: Subject[] = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  searchKey = '';

  constructor(private subjectService: SubjectApiService) {}

  ngOnInit(): void {
    this.loadSubjects();
  }

  loadSubjects(): void {
    const request: PagingRequest = {
      searchKey: this.searchKey,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize
    };

    this.subjectService.getSubjects(request).subscribe((result) => {
      this.subjects = result.items;
      this.totalCount = result.totalCount;
    });
  }

  onPageChange(newPage: number): void {
    this.pageNumber = newPage;
    this.loadSubjects();
  }

  onSearch(): void {
    this.pageNumber = 1;
    this.loadSubjects();
  }

  getTagValue(value: boolean): 'Yes' | 'No' {
    return value ? 'Yes' : 'No';
  }

  deleteSubject(index: number) {
    this.subjects.splice(index, 1);
  }
}
