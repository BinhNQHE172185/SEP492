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
import { searchService } from '../../../service/search/search-service.service';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ContractApiService } from '../../../../apis/contractAPIs/contract-api.service';
import { AcceptanceRecordApiService } from '../../../../apis/acceptanceRecordAPIs/acceptance-api.service';

interface PagingRequest {
  searchKey?: string;
  pageIndex: number;
  pageSize: number;
}

@Component({
  selector: 'app-list-contract',
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

  ],
  templateUrl: './list-acceptance-report.component.html',
  styleUrls: ['./list-acceptance-report.component.scss'],
  providers: [
    MessageService,
    ConfirmationService
  ]
})
export class ListAcceptanceReportComponent implements OnInit {
  report: any = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  searchKey = '';

  isDetail: boolean = true;

  selectedContractId: string | null = null;

  private searchSubscription!: Subscription;

  constructor(private acceptanceService: AcceptanceRecordApiService,
    private searchService: searchService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) { }

  displayAddDialog = false;
  displayDetailDialog = false;

  detailContract: any;
  contractId: string | null = null;

  ngOnInit() {
    this.searchSubscription = this.searchService.searchQuery$.subscribe(
      (query) => {
        this.searchKey = query;
        this.loadContract();
      }
    );
  }

  loadContract(event?: any) {
    if (event) {
      this.pageNumber = Math.floor(event.first / event.rows) + 1;
      this.pageSize = event.rows;
    }

    const request: PagingRequest = {
      pageIndex: this.pageNumber,
      pageSize: this.pageSize,
      searchKey: this.searchKey,
    };
    this.acceptanceService.getAcceptanceRecords(request).subscribe(
      (response) => {
        this.report = response.items;
        this.totalCount = response.totalCount;
      },
      (error) => {
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
    this.contractId = id;
    this.displayDetailDialog = true;
    console.log(id);
  }

  handleCloseDialog(isDetail: boolean) {
    if (isDetail) {
      this.displayDetailDialog = false;
    } else {
      this.displayAddDialog = false;
      this.selectedContractId = null;
    }
    this.loadContract();
  }

  openAddDialog(id?: string) {
    if (id) {
      this.selectedContractId = id;
    }
    else {
      this.selectedContractId = null;
    }
    this.displayAddDialog = true;
  }

  deleteContract(id: any) {
    this.confirmationService.confirm({
      header: 'Xóa dữ liệu',
      message: 'Bạn có chắc chắn muốn xóa? Hành động này là không thể hoàn tác.',
      acceptLabel: 'Đồng ý',
      rejectLabel: 'Hủy',
      accept: () => {
        this.acceptanceService.deleteAcceptanceRecord(id!).subscribe(
          (response) => {
            this.loadContract();
            this.messageService.add({ severity: 'success', summary: 'Thành công', detail: response.message });
          },
          (error) => {
            this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: error.error.message });
          }
        );
      }
    });
  }
}
