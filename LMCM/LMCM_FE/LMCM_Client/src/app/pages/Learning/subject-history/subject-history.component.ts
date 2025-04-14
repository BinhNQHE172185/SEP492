import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { CardModule } from 'primeng/card';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DatePickerModule } from 'primeng/datepicker';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { FileUploadModule } from 'primeng/fileupload';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { TextareaModule } from 'primeng/textarea';
import { ToastModule } from 'primeng/toast';
import { searchService } from '../../service/search/search-service.service';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { TagModule } from 'primeng/tag';
import { LearningMaterialApiService } from '../../../apis/learning-materialAPIs/learning-material-api.service';

interface PagingRequest {
  id: string;
  searchKey?: string;
  pageIndex: number;
  pageSize: number;
}

@Component({
  selector: 'app-subject-history',
  imports: [
    TextareaModule,
    ConfirmDialogModule,
    DatePickerModule,
    ToastModule,
    FileUploadModule,
    DialogModule,
    InputGroupModule,
    FormsModule,
    CommonModule,
    TableModule,
    ButtonModule,
    CardModule,
    InputTextModule,
    CalendarModule,
    DropdownModule,
    InputTextModule,
    TagModule
  ],
  templateUrl: './subject-history.component.html',
  styleUrl: './subject-history.component.scss',
  providers: [ConfirmationService, MessageService]
})
export class SubjectHistoryComponent {
  subjectId: string = '';
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  searchKey = '';
  historyList: any[] = [];

  displayImportDialog: boolean = false;
  uploadedFiles: any[] = [];

  private searchSubscription!: Subscription;

  constructor(
    private learningMaterialService: LearningMaterialApiService,
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
        this.loadHistory();
      }
    );
  }

  loadHistory(event?: any) {
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
    this.learningMaterialService.getHistoryBySubjectId(request).subscribe(
      (response) => {
        this.historyList = response.items;
        this.totalCount = response.totalCount;
      },
      (error) => {
        console.error("Lỗi khi tải danh sách:", error);
      }
    );
  }

  showImportDialog() {
    this.displayImportDialog = true;
  }

  closeDialog() {
    this.displayImportDialog = false;
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

  goToDetail(id: string) {
    const url = `/learning/syllabus/${id}`;
    window.open(url, '_blank');
  }
  goToContractDetail(id: string) {
    const url = `/document/contract/${id}`;
    window.open(url, '_blank');
  }
}
