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
import { ProgressSpinnerModule } from 'primeng/progressspinner';

@Component({
    selector: 'app-expert-create-edit',
    standalone: true,
    imports: [ProgressSpinnerModule, DialogModule, InputTextModule, ButtonModule, CommonModule, FormsModule, FileUploadModule, DatePickerModule, ToastModule],
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
    isLoading: boolean = false;

    constructor(
        private messageService: MessageService,
        private expertService: ContractorApiService
    ) { }

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
        if(this.expert.contractorName.trim().length < 3) {
            this.messageService.add({ severity: 'warn', summary: 'Cảnh báo', detail: 'Tên chuyên gia phải có ít nhất 3 ký tự không bao gồm khoảng trắng.' });
            return;
        }

        if (!this.expert.contractorName) {
            this.messageService.add({ severity: 'warn', summary: 'Cảnh báo', detail: 'Tên chuyên gia không được để trống.' });
            return;
        }
        if (!this.expert.email) {
            this.messageService.add({ severity: 'warn', summary: 'Cảnh báo', detail: 'Email không được để trống.' });
            return;
        }
        if (!this.expert.phoneNumber) {
            this.messageService.add({ severity: 'warn', summary: 'Cảnh báo', detail: 'Số điện thoại không được để trống.' });
            return;
        }
        if (!this.expert.taxCode) {
            this.messageService.add({ severity: 'warn', summary: 'Cảnh báo', detail: 'Mã số thuế không được để trống.' });
            return;
        }
        if (!this.expert.employeeCode) {
            this.messageService.add({ severity: 'warn', summary: 'Cảnh báo', detail: 'Mã nhân viên không được để trống.' });
            return;
        }
        if (!this.expert.idCardNumber) {
            this.messageService.add({ severity: 'warn', summary: 'Cảnh báo', detail: 'Số CMND/CCCD không được để trống.' });
            return;
        }

        this.messageService.add({ severity: 'info', summary: 'Đang tải', detail: 'Vui lòng chờ.' });
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
            this.isLoading = true;
            this.expertService.updateContractor(this.selectedId, reportData).subscribe(
                (response) => {
                    this.isLoading = false;
                    this.messageService.add({ severity: 'success', summary: 'Thành công', detail: response.message });
                    this.closeDialog();
                },
                (error) => {
                    this.isLoading = false;
                    const errors = error.error?.errors;
                    if (errors) {
                        const allMessages = Object.values(errors).flat();
                        allMessages.forEach(msg => {
                            this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: msg as string });
                        });
                    } else {
                        this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: error.error?.message || 'Đã có lỗi xảy ra.' });
                    }
                }
            );
        } else {
            this.expertService.createContractor(reportData).subscribe(
                (response) => {
                    this.isLoading = false;
                    this.messageService.add({ severity: 'success', summary: 'Thành công', detail: response.message });
                    this.closeDialog();
                },
                (error) => {
                    this.isLoading = false;
                    const errors = error.error?.errors;
                    if (errors) {
                        const allMessages = Object.values(errors).flat();
                        allMessages.forEach(msg => {
                            this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: msg as string });
                        });
                    } else {
                        this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: error.error?.message || 'Đã có lỗi xảy ra.' });
                    }
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
