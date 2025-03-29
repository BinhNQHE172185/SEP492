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
import { TemplateCreateEditComponent } from '../template-create-edit/template-create-edit.component';
import { TemplateDetailComponent } from '../template-detail/template-detail.component';
import { DocumentTemplateApiService } from '../../../../apis/templateAPIs/template-api.service';

interface PagingRequest {
  searchKey?: string;
  pageIndex: number;
  pageSize: number;
}

@Component({
  selector: 'app-list-template',
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
    TemplateCreateEditComponent,
    // TemplateDetailComponent
  ],
  templateUrl: './list-template.component.html',
  styleUrls: ['./list-template.component.scss'],
  providers: [
    MessageService,
    ConfirmationService
  ]
})
export class ListTemplateComponent implements OnInit {
  templates: any = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  searchKey = '';

  isDetail: boolean = true;

  selectedId: string | null = null;

  private searchSubscription!: Subscription;

  constructor(private templateService: DocumentTemplateApiService,
    private searchService: searchService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) { }

  displayAddDialog = false;
  displayDetailDialog = false;

  detailTemplate: any;
  templateId: string | null = null;

  ngOnInit() {
    this.searchSubscription = this.searchService.searchQuery$.subscribe(
      (query) => {
        this.searchKey = query;
        this.loadTemplate();
      }
    );
  }

  loadTemplate(event?: any) {
    if (event) {
      this.pageNumber = Math.floor(event.first / event.rows) + 1;
      this.pageSize = event.rows;
    }

    const request: PagingRequest = {
      pageIndex: this.pageNumber,
      pageSize: this.pageSize,
      searchKey: this.searchKey,
    };
    this.templateService.getTemplates(request).subscribe(
      (response) => {
        this.templates = response.items;
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
    this.templateId = id;
    this.displayDetailDialog = true;
    console.log(id);
  }

  handleCloseDialog(isDetail: boolean) {
    if (isDetail) {
      this.displayDetailDialog = false;
    } else {
      this.displayAddDialog = false;
      this.selectedId = null;
    }
    this.loadTemplate();
  }

  openAddDialog(id?: string) {
    if (id) {
      this.selectedId = id;
    }
    else {
      this.selectedId = null;
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
        this.templateService.deleteTemplate(id!).subscribe(
          (response) => {
            this.loadTemplate();
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
