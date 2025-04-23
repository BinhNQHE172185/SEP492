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
import { ContractCreateEditComponent } from '../contract-create-edit/contract-create-edit.component';
import { ContractDetailComponent } from '../contract-detail/contract-detail.component';
import { ActivatedRoute } from '@angular/router';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

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
    ContractCreateEditComponent,
    ContractDetailComponent,
    ProgressSpinnerModule
  ],
  templateUrl: './list-contract.component.html',
  styleUrls: ['./list-contract.component.scss'],
  providers: [
    MessageService,
    ConfirmationService
  ]
})
export class ListContractComponent implements OnInit {
  contracts: any = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  searchKey = '';

  isDetail: boolean = true;

  selectedContractId: string | null = null;

  private searchSubscription!: Subscription;

  constructor(private contractService: ContractApiService,
    private searchService: searchService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private route: ActivatedRoute
  ) { }

  displayAddDialog = false;
  displayDetailDialog = false;

  detailContract: any;
  contractId: string | null = null;
  isLoading: boolean = false;

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.contractId = id;
        this.displayDetailDialog = true;
      }
    });
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
    this.contractService.getContract(request).subscribe(
      (response) => {
        this.contracts = response.items;
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
        this.isLoading = true;
        this.contractService.deleteContract(id!).subscribe(
          (response) => {
            this.loadContract();
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
  goToExpertDetail(id: string) {
    const url = `/document/contractor/${id}`;
    window.open(url, '_blank');
  }

  goToProposalDetail(id: string) {
    const url = `/document/report/${id}`;
    window.open(url, '_blank');
  }
}
