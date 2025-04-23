import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputGroupModule } from 'primeng/inputgroup';
import { CardModule } from 'primeng/card';
import { FormsModule } from '@angular/forms';
import { TagModule } from 'primeng/tag';
import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { FileUploadModule } from 'primeng/fileupload';
import { TextareaModule } from 'primeng/textarea';
import { CalendarModule } from 'primeng/calendar';
import { Subscription } from 'rxjs';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ReportDetailComponent } from '../report-detail/report-detail.component';
import { BudgetApiService } from '../../../../apis/budgetProposalAPIs/budget-api.service';
import { searchService } from '../../../service/search/search-service.service';
import { ReportCreateEditComponent } from '../report-create-edit/report-create-edit.component';
import { ActivatedRoute } from '@angular/router';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

interface PagingRequest {
  searchKey?: string;
  pageIndex: number;
  pageSize: number;
}

@Component({
  selector: 'app-list-report',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    InputGroupModule,
    CardModule,
    ConfirmDialogModule,
    ToastModule,
    FileUploadModule,
    DialogModule,
    FormsModule,
    TagModule,
    TextareaModule,
    CalendarModule,
    FormsModule,
    ToastModule,
    ReportDetailComponent,
    ReportCreateEditComponent,
    ProgressSpinnerModule
  ],
  templateUrl: './list-report.component.html',
  styleUrls: ['./list-report.component.scss'],
  providers: [
    MessageService,
    ConfirmationService
  ]
})
export class ListReportComponent implements OnInit {
  reports: any = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  searchKey = '';

  isDetail: boolean = true;

  selectedFile: File | null = null;
  selectedReportId: string | null = null;

  private searchSubscription!: Subscription;

  constructor(private reportService: BudgetApiService,
    private searchService: searchService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private route: ActivatedRoute
  ) { }

  displayAddDialog = false;
  displayDetailDialog = false;

  detailReport: any;
  proposalId: string | null = null;
  isLoading: boolean = false;

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.proposalId = id;
        this.displayDetailDialog = true;
      }
    });
    this.searchSubscription = this.searchService.searchQuery$.subscribe(
      (query) => {
        this.searchKey = query;
        this.loadBudget();
      }
    );
  }

  loadBudget(event?: any) {
    if (event) {
      this.pageNumber = Math.floor(event.first / event.rows) + 1;
      this.pageSize = event.rows;
    }

    const request: PagingRequest = {
      pageIndex: this.pageNumber,
      pageSize: this.pageSize,
      searchKey: this.searchKey,
    };
    this.reportService.getBudget(request).subscribe(
      (response) => {
        this.reports = response.items;
        this.totalCount = response.totalCount;
      },
      (error) => {
        console.error("Lỗi khi tải danh sách môn học:", error);
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

  openDetailDialog(id: string) {
    this.proposalId = id;
    this.displayDetailDialog = true;
  }

  handleCloseDialog(isDetail: boolean) {
    if (isDetail) {
      this.displayDetailDialog = false;
    } else {
      this.displayAddDialog = false;
      this.selectedReportId = null;
    }
    this.loadBudget();
  }

  openAddDialog(id?: string) {
    if (id) {
      this.selectedReportId = id;
    }
    else {
      this.selectedReportId = null;
    }
    this.displayAddDialog = true;
  }

  deleteBudgetProposal(id: any) {
    console.log("true")
    const authorId = localStorage.getItem("userId");
    this.confirmationService.confirm({
      header: 'Xóa dữ liệu',
      message: 'Bạn có chắc chắn muốn xóa? Hành động này là không thể hoàn tác.',
      acceptLabel: 'Đồng ý',
      rejectLabel: 'Hủy',
      accept: () => {
        this.isLoading = true;
        this.reportService.deleteBudget(id, authorId!).subscribe(
          (response) => {
            this.loadBudget();
            this.isLoading = false;
            this.messageService.add({ severity: 'success', summary: 'Thành công', detail: response.message });
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