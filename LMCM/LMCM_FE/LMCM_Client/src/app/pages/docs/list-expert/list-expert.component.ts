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

interface Expert {
  name: string;
  email: string;
  phone: string;
  taxNumber: string;
  address: string;
}

@Component({
  selector: 'app-list-expert',
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
    CalendarModule
  ],
  templateUrl: './list-expert.component.html',
  styleUrls: ['./list-expert.component.scss'] // <-- Ensure this matches your file name
})
export class ListExpertComponent implements OnInit {
  // --------------------------
  // Mock Data
  // --------------------------
  experts: Expert[] = [
    {
      name: 'Dr. John Smith',
      email: 'john@example.com',
      phone: '0912345678',
      taxNumber: 'MST123456789',
      address: '123 Đường Nguyễn Huệ, Quận 1, TP.HCM'
    },
    {
      name: 'Dr. Sarah Johnson',
      email: 'sarah@example.com',
      phone: '0987654321',
      taxNumber: 'MST987654321',
      address: '456 Đường Lê Lợi, Quận 3, TP.HCM'
    },
    {
      name: 'Dr. Michael Brown',
      email: 'michael@example.com',
      phone: '0909123456',
      taxNumber: 'MST555999222',
      address: '789 Đường Pasteur, Quận 1, TP.HCM'
    }
  ];

  // --------------------------
  // Dialog States
  // --------------------------
  displayAddDialog = false;
  displayDetailDialog = false;
  displayEditDialog = false;

  // --------------------------
  // Add Dialog Fields
  // --------------------------
  addName = '';
  addEmail = '';
  addPhone = '';
  addTaxNumber = '';
  addAddress = '';

  // --------------------------
  // Edit Dialog Fields
  // --------------------------
  editName = '';
  editEmail = '';
  editPhone = '';
  editTaxNumber = '';
  editAddress = '';
  editIndex = -1; // Keep track of which item we are editing

  // --------------------------
  // Detail Dialog Fields
  // --------------------------
  detailExpert: Expert | null = null;

  // --------------------------
  // Search
  // --------------------------
  searchKey = '';
  filteredExperts: Expert[] = [];

  ngOnInit() {
    // Initialize filtered list
    this.filteredExperts = [...this.experts];
  }

  // --------------------------
  // Search Handler
  // --------------------------
  onSearchChange() {
    const key = this.searchKey.toLowerCase().trim();
    this.filteredExperts = this.experts.filter((expert) =>
      expert.name.toLowerCase().includes(key) ||
      expert.email.toLowerCase().includes(key) ||
      expert.phone.toLowerCase().includes(key) ||
      expert.taxNumber.toLowerCase().includes(key) ||
      expert.address.toLowerCase().includes(key)
    );
  }

  // --------------------------
  // Open Dialogs
  // --------------------------
  openAddDialog() {
    // Reset fields
    this.addName = '';
    this.addEmail = '';
    this.addPhone = '';
    this.addTaxNumber = '';
    this.addAddress = '';
    this.displayAddDialog = true;
  }

  openDetailDialog(expert: Expert) {
    // Show detail data
    this.detailExpert = expert;
    this.displayDetailDialog = true;
  }

  openEditDialog(index: number) {
    this.editIndex = index;
    const expert = this.filteredExperts[index];
    this.editName = expert.name;
    this.editEmail = expert.email;
    this.editPhone = expert.phone;
    this.editTaxNumber = expert.taxNumber;
    this.editAddress = expert.address;
    this.displayEditDialog = true;
  }

  // --------------------------
  // Close Dialogs
  // --------------------------
  closeDialog(dialogType: string) {
    if (dialogType === 'add') {
      this.displayAddDialog = false;
    } else if (dialogType === 'detail') {
      this.displayDetailDialog = false;
    } else if (dialogType === 'edit') {
      this.displayEditDialog = false;
    }
  }

  // --------------------------
  // Add Dialog Save
  // --------------------------
  saveNewExpert() {
    const newExpert: Expert = {
      name: this.addName,
      email: this.addEmail,
      phone: this.addPhone,
      taxNumber: this.addTaxNumber,
      address: this.addAddress
    };
    this.experts.push(newExpert);
    this.onSearchChange(); // refresh filtered data
    this.displayAddDialog = false;
  }

  // --------------------------
  // Edit Dialog Save
  // --------------------------
  saveEditedExpert() {
    if (this.editIndex < 0 || this.editIndex >= this.filteredExperts.length) {
      return;
    }

    // Update the original array item
    const globalIndex = this.experts.indexOf(this.filteredExperts[this.editIndex]);
    this.experts[globalIndex] = {
      name: this.editName,
      email: this.editEmail,
      phone: this.editPhone,
      taxNumber: this.editTaxNumber,
      address: this.editAddress
    };

    // Re-filter after edit
    this.onSearchChange();
    this.displayEditDialog = false;
  }

  // --------------------------
  // Delete
  // --------------------------
  deleteExpert(index: number) {
    const globalIndex = this.experts.indexOf(this.filteredExperts[index]);
    this.experts.splice(globalIndex, 1);
    this.onSearchChange();
  }
}
