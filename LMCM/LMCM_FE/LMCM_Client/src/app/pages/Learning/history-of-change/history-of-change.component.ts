import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { FormsModule } from '@angular/forms';
import { InputGroupModule } from 'primeng/inputgroup';
import { Subscription } from 'rxjs';
import { LearningMaterialApiService } from '../../../apis/learning-materialAPIs/learning-material-api.service';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { FileUploadModule } from 'primeng/fileupload';
import { DialogModule } from 'primeng/dialog';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { DatePickerModule } from 'primeng/datepicker';
import { TextareaModule } from 'primeng/textarea';
import { ContractApiService } from '../../../apis/contractAPIs/contract-api.service';
import { SyllabusApiService } from '../../../apis/syllabusAPIs/syllabus-api.service';
import { HistoryOfChangeApiService } from '../../../apis/historyAPIs/history-api';
import { searchService } from '../../service/search/search-service.service';
import { AutoCompleteModule } from 'primeng/autocomplete';

@Component({
    standalone: true,
    imports: [AutoCompleteModule, TextareaModule, ConfirmDialogModule, DatePickerModule, ToastModule, FileUploadModule, DialogModule, InputGroupModule, FormsModule, CommonModule, TableModule, ButtonModule, CardModule, InputTextModule, CalendarModule, DropdownModule, InputTextModule],
    selector: 'app-history-of-change',
    templateUrl: './history-of-change.component.html',
    styleUrls: ['./history-of-change.component.scss'],
    providers: [ConfirmationService, MessageService]
})
export class HistoryOfChangeComponent implements OnInit, OnDestroy {
    historyList: any[] = [];
    totalCount = 0;
    pageSize = 10;
    pageIndex = 1;
    searchKey = '';
    private searchSubscription!: Subscription;
    displayDetailDialog = false;
    displayAddDialog = false;
    selectedItem: any = {};
    newHistory: any = {};
    displayEditDialog: boolean = false;

    editHistory: any = {};

    contract: any[] = [];
    syllabus: any[] = [];
    suggestedStartTerms: string[] = [];
    filteredStartTerms: string[] = [];

    constructor(
        private searchService: searchService,
        private historyService: HistoryOfChangeApiService,
        private confirmationService: ConfirmationService,
        private messageService: MessageService,
        private contractService: ContractApiService,
        private syllabusService: SyllabusApiService
    ) { }

    changeTypeOptions = [
        { label: 'Xây mới', value: 'Xây mới' },
        { label: 'Điều chỉnh', value: 'Điều chỉnh' },
    ];

    loadData() {
        this.contractService.getContractList().subscribe((response) => {
            this.contract = response;
        });

        this.syllabusService.getSyllabusList().subscribe((response) => {
            this.syllabus = response;
        });
    }

    ngOnInit(): void {
        this.searchSubscription = this.searchService.searchQuery$.subscribe(
            (query) => {
                this.searchKey = query;
                this.loadHistory();
            }
        );
        this.loadData();
    }

    onSearchChange(query: string) {
        this.searchService.updateSearchQuery(query);
    }

    getUserIdFromLocalStorage(): string | null {
        return localStorage.getItem('userId');
    }

    loadHistory(event?: any) {
        if (event) {
            this.pageIndex = Math.floor(event.first / event.rows) + 1;
            this.pageSize = event.rows || this.pageSize;
        }

        const request = {
            searchKey: this.searchKey.trim(),
            pageIndex: this.pageIndex,
            pageSize: this.pageSize
        };

        this.historyService.getLearningMaterialHistory(request).subscribe({
            next: (response) => {
                this.historyList = response.items;
                this.totalCount = response.totalCount;
                this.suggestedStartTerms = Array.from(
                    new Set(
                        (response.items || [])
                            .map(item => item.startTerm?.trim())
                            .filter(term => !!term)
                    )
                );
            },
            error: (error) => {
                console.error('Lỗi khi gọi API:', error);
                this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Không thể tải dữ liệu' });
            }
        });
    }

    filterStartTerm(event: any) {
        const query = event.query.toLowerCase();
        this.filteredStartTerms = this.suggestedStartTerms.filter(term =>
            term.toLowerCase().includes(query)
        );
    }

    paginate(event: any) {
        this.loadHistory(event);
    }
    showDetail(id: any) {
        this.historyService.getHistoryById(id).subscribe(
            (response) => {
                this.newHistory = response;
                const selectedSyllabus = this.syllabus.find(s => s.syllabusId === this.newHistory.syllabusId);
                this.newHistory.syllabus = selectedSyllabus;
                if (this.newHistory.completionDate) {
                    this.newHistory.completionDate = new Date(this.newHistory.completionDate);
                }
            },
            (error) => {
                this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: error.message });
            }
        );

        this.displayDetailDialog = true;
    }
    closeDetailDialog() {
        this.displayDetailDialog = false;
    }

    editDetail(item: any) {
        this.editHistory = {
            ...item,
            completionDate: item.completionDate ? new Date(item.completionDate) : null,
        };
        this.displayEditDialog = true;
    }

    updateHistory() {

        this.displayEditDialog = false;
    }

    confirmDelete(id: any) {
        this.confirmationService.confirm({
            message: 'Bạn có chắc chắn muốn xóa bản ghi này?',
            header: 'Xác nhận',
            accept: () => {
                this.deleteItem(id);
            }
        });
    }

    deleteItem(id: any) {
        this.historyService.deleteLearningMaterialHistory(id).subscribe({
            next: () => {
                this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Đã xóa lịch sử thay đổi' });
                this.loadHistory();
            },
            error: (error) => {
                this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Không thể xóa lịch sử thay đổi' });
            }
        });
    }

    openAddDialog(id?: string) {
        this.loadData();
        if (id) {
            this.historyService.getHistoryById(id).subscribe(
                (response) => {
                    this.newHistory = response;
                    const selectedSyllabus = this.syllabus.find(s => s.syllabusId === this.newHistory.syllabusId);
                    this.newHistory.syllabus = selectedSyllabus;
                    this.newHistory.changeType = response.changeType;
                    if (this.newHistory.completionDate) {
                        this.newHistory.completionDate = new Date(this.newHistory.completionDate);
                    }
                },
                (error) => {
                    this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: error.message });
                }
            );
        } else {
            this.newHistory = {};
        }
        this.displayAddDialog = true;
    }

    addHistory(id?: string) {
        const request = {
            contractId: this.newHistory.contractId || null,
            changeType: this.newHistory.changeType.toString(),
            changeDescription: this.newHistory.changeDescription || '',
            completionDate: this.newHistory.completionDate.toLocaleDateString('en-CA'),
            startTerm: this.newHistory.startTerm || '',
            syllabusId: this.newHistory.syllabus?.syllabusId || '',
            courseCode: this.newHistory.syllabus?.courseCode || '',
        };
        if (id) {
            this.historyService.updateLearningMaterialHistory(id, request).subscribe({
                next: () => {
                    this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Đã cập nhật lịch sử thay đổi' });
                    this.displayAddDialog = false;
                    this.loadHistory();
                },
                error: (error) => {
                    this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Không thể cập nhật lịch sử thay đổi' });
                }
            });
        } else {
            this.historyService.createLearningMaterialHistory(request).subscribe({
                next: () => {
                    this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Đã thêm lịch sử thay đổi' });
                    this.displayAddDialog = false;
                    this.loadHistory();
                },
                error: (error) => {
                    console.error('Lỗi khi thêm lịch sử thay đổi:', error);
                    this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Không thể thêm lịch sử thay đổi' });
                }
            });
        }
    }

    ngOnDestroy(): void {
        if (this.searchSubscription) {
            this.searchSubscription.unsubscribe();
        }
    }
}
