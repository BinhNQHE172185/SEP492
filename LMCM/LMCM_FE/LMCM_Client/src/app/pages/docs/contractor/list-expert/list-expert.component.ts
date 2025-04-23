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
import { ContractorApiService } from '../../../../apis/contractorAPIs/contractor-api.service';
import { ConfirmationService, MessageService } from 'primeng/api';
import { searchService } from '../../../service/search/search-service.service';
import { Subscription } from 'rxjs';
import { ExpertCreateEditComponent } from '../expert-create-edit/expert-create-edit.component';
import { ExpertDetailComponent } from '../expert-detail/expert-detail.component';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

interface PagingRequest {
    searchKey?: string;
    pageIndex: number;
    pageSize: number;
}

@Component({
    selector: 'app-list-expert',
    standalone: true,
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
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
        ExpertCreateEditComponent,
        ExpertDetailComponent,
        ProgressSpinnerModule
    ],
    providers: [MessageService, ConfirmationService],
    templateUrl: './list-expert.component.html',
    styleUrls: ['./list-expert.component.scss']
})
export class ListExpertComponent implements OnInit {
    expert: any = [];
    totalCount = 0;
    pageNumber = 1;
    pageSize = 10;
    searchKey = '';

    isDetail: boolean = true;

    selectedFile: File | null = null;
    selectedId: string | null = null;

    private searchSubscription!: Subscription;

    constructor(
        private ExpertService: ContractorApiService,
        private searchService: searchService,
        private messageService: MessageService,
        private confirmationService: ConfirmationService,
        private route: ActivatedRoute
    ) {}

    displayAddDialog = false;
    displayDetailDialog = false;

    detailExpert: any;
    expertId: string | null = null;
    isLoading: boolean = false;

    ngOnInit() {
        this.route.paramMap.subscribe(params => {
            const id = params.get('id');
            if (id) {
              this.expertId = id;
              this.displayDetailDialog = true;
            }
          });
        this.searchSubscription = this.searchService.searchQuery$.subscribe((query) => {
            this.searchKey = query;
            this.loadExpert();
        });
    }

    loadExpert(event?: any) {
        if (event) {
            this.pageNumber = Math.floor(event.first / event.rows) + 1;
            this.pageSize = event.rows;
        }

        const request: PagingRequest = {
            pageIndex: this.pageNumber,
            pageSize: this.pageSize,
            searchKey: this.searchKey
        };
        this.ExpertService.getContractors(request).subscribe(
            (response) => {
                this.expert = response.items;
                this.totalCount = response.totalCount;
            },
            (error) => {
                console.error('Lỗi khi tải danh sách môn học:', error);
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
        this.expertId = id;
        this.displayDetailDialog = true;
    }

    handleCloseDialog(isDetail: boolean) {
        if (isDetail) {
            this.displayDetailDialog = false;
        } else {
            this.displayAddDialog = false;
            this.selectedId = null;
        }
        this.loadExpert();
    }

    openAddDialog(id?: string) {
        if (id) {
            this.selectedId = id;
        } else {
            this.selectedId = null;
        }
        this.displayAddDialog = true;
    }

    deleteE(id: any) {
        console.log('true');
        const authorId = localStorage.getItem('userId');
        this.confirmationService.confirm({
            header: 'Xóa dữ liệu',
            message: 'Bạn có chắc chắn muốn xóa? Hành động này là không thể hoàn tác.',
            acceptLabel: 'Đồng ý',
            rejectLabel: 'Hủy',
            accept: () => {
                this.isLoading = true;
                this.ExpertService.deleteContractor(id).subscribe(
                    (response) => {
                        this.loadExpert();
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
