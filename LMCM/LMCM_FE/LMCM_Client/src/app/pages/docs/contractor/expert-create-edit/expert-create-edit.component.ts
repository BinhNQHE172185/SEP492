import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { DialogModule } from 'primeng/dialog';
import { FileUploadModule } from 'primeng/fileupload';
import { InputTextModule } from 'primeng/inputtext';
import { BudgetApiService } from '../../../../apis/budgetProposalAPIs/budget-api.service';
import { DatePickerModule } from 'primeng/datepicker';
import { ToastModule } from 'primeng/toast';
import { ContractApiService } from '../../../../apis/contractAPIs/contract-api.service';
import { ContractorApiService } from '../../../../apis/contractorAPIs/contractor-api.service';

@Component({
    selector: 'app-expert-create-edit',
    standalone: true,
    imports: [DialogModule, InputTextModule, ButtonModule, CommonModule, FormsModule, FileUploadModule, DatePickerModule, ToastModule],
    templateUrl: './expert-create-edit.component.html',
    styleUrl: './expert-create-edit.component.scss',
    providers: [ConfirmationService, MessageService]
})
export class ExpertCreateEditComponent {
    @Input() displayAddDialog: boolean = false;
    @Input() selectedId: string | null = null;
    @Output() closeDialogEvent = new EventEmitter<void>();

    calendarValue: any = null;

    expert: any;
    budgetProposal: any;

    constructor(
        private messageService: MessageService,
        private expertService: ContractorApiService
    ) {}

    ngOnChanges() {
        if (this.selectedId) {
            this.expertService.getContractorDetail(this.selectedId).subscribe(
                (response) => {
                    if (response) {
                        this.expert = response;
                    }
                },
                (error) => {
                    this.messageService.add({
                        severity: 'error',
                        summary: 'Thất bại',
                        detail: error?.error?.message || 'Đã xảy ra lỗi khi tải dữ liệu.'
                    });
                }
            );
        } else {
            this.resetForm();
        }
    }

    save() {
        const reportData = {
            contractorName: this.expert.contractorName,
            email: this.expert.email,
            address: this.expert.address,
            phoneNumber: this.expert.phoneNumber,
            taxCode: this.expert.taxCode,
            employeeCode: this.expert.employeeCode,
            idCardNumber: this.expert.idCardNumber,
            idIssuedPlace: this.expert.idIssuedPlace,
            position: this.expert.position,
            bankAccountNumber: this.expert.bankAccountNumber,
            bankName: this.expert.bankName
           
        };

        if (this.selectedId) {
            this.expertService.updateContractor(this.selectedId, reportData).subscribe(
                (response) => {
                    this.messageService.add({ severity: 'success', summary: 'Thành công', detail: response.message });
                    this.closeDialog();
                },
                (error) => {
                    this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: error.error.message });
                }
            );
        } else {
            this.messageService.add({ severity: 'info', summary: 'Đang tải', detail: 'Vui lòng chờ.' });
            this.expertService.createContractor(reportData).subscribe(
                (response) => {
                    this.messageService.add({ severity: 'success', summary: 'Thành công', detail: response.message });
                    this.closeDialog();
                },
                (error) => {
                    this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: error.error.message });
                }
            );
        }
    }

    resetForm() {
        this.expert = {
            contractorName: '',
            email: '',
            idIssuedPlace: '',
            position: '',
            bankAccountNumber: '',
            bankName: '',
            address: '',
            phoneNumber: '',
            taxCode: '',
            employeeCode: '',
            idCardNumber: ''
        };
    }

    closeDialog() {
        this.selectedId = '';
        this.displayAddDialog = false;
        this.closeDialogEvent.emit();
    }
}
