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
import { ContractorApiService } from '../../../apis/contractorAPIs/contractor-api.service';
import {ConfirmationService, MessageService } from 'primeng/api';

interface Expert {
    id: string;
    contractorName: string;
    address: string;
    phoneNumber: string;
    taxCode: string;
    email: string;
    employeeCode: string;
    idCardNumber: string;
    idIssuedPlace: string;
    position: string;
    bankAccountNumber: string;
    bankName: string;
}

@Component({
    selector: 'app-list-expert',
    standalone: true,
    imports: [CommonModule, TableModule, ButtonModule, InputTextModule, InputGroupModule, CardModule, ConfirmDialogModule, ToastModule, FileUploadModule, DialogModule, FormsModule, TagModule, TextareaModule, CalendarModule],
    providers: [MessageService,ConfirmationService],
    templateUrl: './list-expert.component.html',
    styleUrls: ['./list-expert.component.scss']
})
export class ListExpertComponent implements OnInit {
    experts: Expert[] = [];
    filteredExperts: Expert[] = [];
    searchKey = '';

    displayAddDialog = false;
    displayDetailDialog = false;
    displayEditDialog = false;

    addContractorName = '';
    addPhoneNumber = '';
    addTaxCode = '';
    addEmail = '';
    addEmployeeCode = '';
    addIdCardNumber = '';
    addIdIssuedPlace = '';
    addPosition = '';
    addBankAccountNumber = '';
    addBankName = '';
    addAddress = '';

    editId = '';
    editContractorName = '';
    editAddress = '';
    editPhoneNumber = '';
    editTaxCode = '';
    editEmail = '';
    editEmployeeCode = '';
    editIdCardNumber = '';
    editIdIssuedPlace = '';
    editPosition = '';
    editBankAccountNumber = '';
    editBankName = '';
    editStatus = '';

    detailExpert: Expert | null = null;

    constructor(
        private contractorService: ContractorApiService,
        private messageService: MessageService,
        private confirmationService: ConfirmationService
    ) {}

    ngOnInit() {
        this.loadExperts();
    }

    loadExperts() {
        const request = { searchKey: '', pageIndex: 1, pageSize: 10 };

        this.contractorService.getContractors(request).subscribe(
            (response) => {
                this.experts = response.items;
                this.filteredExperts = [...this.experts];
            },
            (error) => {}
        );
    }

    onSearchChange() {
        const key = this.searchKey.toLowerCase().trim();
        this.filteredExperts = this.experts.filter(
            (expert) =>
                expert.contractorName.toLowerCase().includes(key) ||
                expert.email.toLowerCase().includes(key) ||
                expert.phoneNumber.toLowerCase().includes(key) ||
                expert.taxCode.toLowerCase().includes(key) ||
                expert.address.toLowerCase().includes(key)
        );
    }

    openAddDialog() {
        this.addContractorName = '';
        this.addPhoneNumber = '';
        this.addTaxCode = '';
        this.addEmail = '';
        this.addEmployeeCode = '';
        this.addIdCardNumber = '';
        this.addIdIssuedPlace = '';
        this.addPosition = '';
        this.addBankAccountNumber = '';
        this.addBankName = '';
        this.addAddress = '';
        this.displayAddDialog = true;
    }

    openDetailDialog(expertId: string) {
        this.contractorService.getContractorDetail(expertId).subscribe(
            (response) => {
                this.detailExpert = response;
                this.displayDetailDialog = true;
            },
            (error) => {}
        );
    }

    openEditDialog(expertId: any) {
        if (!expertId || (typeof expertId !== 'string' && typeof expertId !== 'number')) {
            return;
        }
        this.editId = expertId.toString();

        this.contractorService.getContractorDetail(expertId).subscribe(
            (response) => {
                this.editContractorName = response.contractorName;
                this.editAddress = response.address;
                this.editPhoneNumber = response.phoneNumber;
                this.editTaxCode = response.taxCode;
                this.editEmail = response.email;
                this.editEmployeeCode = response.employeeCode;
                this.editIdCardNumber = response.idCardNumber;
                this.editIdIssuedPlace = response.idIssuedPlace;
                this.editPosition = response.position;
                this.editBankAccountNumber = response.bankAccountNumber;
                this.editBankName = response.bankName;
                this.editStatus = response.status ?? 'Active';

                this.displayEditDialog = true;
            },
            (error) => {}
        );
    }

    closeDialog(dialogType: string) {
        if (dialogType === 'add') this.displayAddDialog = false;
        if (dialogType === 'detail') this.displayDetailDialog = false;
        if (dialogType === 'edit') this.displayEditDialog = false;
    }

    saveNewExpert() {
        const newExpert = {
            contractorName: this.addContractorName,
            phoneNumber: this.addPhoneNumber,
            taxCode: this.addTaxCode,
            email: this.addEmail,
            employeeCode: this.addEmployeeCode,
            idCardNumber: this.addIdCardNumber,
            idIssuedPlace: this.addIdIssuedPlace,
            position: this.addPosition,
            bankAccountNumber: this.addBankAccountNumber,
            bankName: this.addBankName,
            address: this.addAddress
        };

        this.contractorService.createContractor(newExpert).subscribe(
            (response) => {
                this.loadExperts();
                this.displayAddDialog = false;
                this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Chuyên gia đã được thêm' });
            },
            (error) => {}
        );
    }

    confirmDelete(id: any) {
      this.confirmationService.confirm({
          message: 'Bạn có chắc muốn xoá nhà thầu này?',
          header: 'Xác nhận xoá',
          acceptLabel: 'Xác nhận',
          rejectLabel: 'Hủy',
          accept: () => {
              this.deleteContractor(id);
          }
      });
  }
  
  deleteContractor(id: any) {
    this.contractorService.deleteContractor(id).subscribe({
        next: () => {
            
            this.experts = this.experts.filter(c => c.id !== id);
            this.filteredExperts = [...this.experts];

            this.messageService.add({ 
                severity: 'success', 
                summary: 'Thành công', 
                detail: 'Xóa nhà thầu thành công' 
            });
            this.loadExperts();
        },
        error: (err) => {
            console.error('Lỗi xoá nhà thầu:', err);
            this.messageService.add({ 
                severity: 'error', 
                summary: 'Lỗi', 
                detail: 'Xóa nhà thầu thất bại' 
            });
        }
    });
}


    saveEditedExpert() {
        if (!this.editContractorName || !this.editEmail) {
            return;
        }

        this.contractorService
            .updateContractor(this.editId, {
                id: this.editId,
                contractorName: this.editContractorName,
                address: this.editAddress,
                phoneNumber: this.editPhoneNumber,
                taxCode: this.editTaxCode,
                email: this.editEmail,
                employeeCode: this.editEmployeeCode,
                idCardNumber: this.editIdCardNumber,
                idIssuedPlace: this.editIdIssuedPlace,
                position: this.editPosition,
                bankAccountNumber: this.editBankAccountNumber,
                status: this.editStatus ?? 'Active',
                bankName: this.editBankName
            })
            .subscribe({
                next: (response) => {
                    this.loadExperts();
                    this.displayEditDialog = false;
                    this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Chuyên gia đã được cập nhật' });
                },
                error: (err) => {}
            });
    }
}
