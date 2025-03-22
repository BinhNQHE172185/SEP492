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

interface Contract {
  courseCode: string;   // Mã môn
  courseName: string;   // Tên môn
  contractNo: string;   // Số hợp đồng
  contractDate: string;   
  startDate: string;    // Ngày bắt đầu
  contractor: string;   // Bên nhận khoán
  endDate: string;      // Ngày kết thúc (hoặc thời gian hiệu lực)
  cost: string;         // Chi phí
  content: string;      // Nội dung hợp đồng
  fileName?: string;    // File hợp đồng
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
    CalendarModule
  ],
  templateUrl: './list-contract.component.html',
  styleUrls: ['./list-contract.component.scss']
})
export class ListContractComponent implements OnInit {
  // --------------------------
  // Mock Data
  // --------------------------
  contracts: Contract[] = [
    {
      courseCode: 'MON001',
      courseName: 'Toán học đại cương',
      contractNo: 'HD2024/001',
      contractDate: '2024-01-01',
      startDate: '2024-01-01',
      contractor: 'Nguyễn Văn A',
      endDate: '2024-06-30',
      cost: '50,000,000 VND',
      content: 'Hợp đồng xây dựng học liệu cho môn Toán học đại cương.',
      fileName: 'HD2024-001.pdf'
    },
    {
      courseCode: 'MON002',
      courseName: 'Vật lý đại cương',
      contractNo: 'HD2024/002',
      contractDate: '2024-01-01',
      startDate: '2024-01-05',
      contractor: 'Trần Thị B',
      endDate: '2024-07-01',
      cost: '60,000,000 VND',
      content: 'Hợp đồng xây dựng học liệu cho môn Vật lý đại cương.'
    },
    {
      courseCode: 'MON003',
      courseName: 'Hóa học cơ bản',
      contractNo: 'HD2024/003',
      contractDate: '2024-01-01',
      startDate: '2024-02-10',
      contractor: 'Phạm Thị C',
      endDate: '2024-08-15',
      cost: '55,000,000 VND',
      content: 'Hợp đồng xây dựng học liệu cho môn Hóa học cơ bản.',
      fileName: 'HD2024-003.pdf'
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
  addCourseCode = '';
  addCourseName = '';
  addContractNo = '';
  addContractDate: Date | null = null;
  addStartDate: Date | null = null;
  addContractor = '';
  addEndDate: Date | null = null;
  addCost = '';
  addContent = '';
  addFileName = '';

  // --------------------------
  // Edit Dialog Fields
  // --------------------------
  editCourseCode = '';
  editCourseName = '';
  editContractNo = '';
  editContractDate: Date | null = null;
  editStartDate: Date | null = null;
  editContractor = '';
  editEndDate: Date | null = null;
  editCost = '';
  editContent = '';
  editFileName = '';
  editIndex = -1; // Keep track of which item we are editing

  // --------------------------
  // Detail Dialog Fields
  // --------------------------
  detailContract: Contract | null = null;

  // --------------------------
  // Search
  // --------------------------
  searchKey = '';
  filteredContracts: Contract[] = [];

  ngOnInit() {
    // Initialize filtered list
    this.filteredContracts = [...this.contracts];
  }

  // --------------------------
  // Search Handler
  // --------------------------
  onSearchChange() {
    const key = this.searchKey.toLowerCase().trim();
    this.filteredContracts = this.contracts.filter((contract) =>
      contract.courseCode.toLowerCase().includes(key) ||
      contract.courseName.toLowerCase().includes(key) ||
      contract.contractNo.toLowerCase().includes(key) ||
      contract.contractor.toLowerCase().includes(key) ||
      contract.cost.toLowerCase().includes(key) ||
      contract.content.toLowerCase().includes(key)
    );
  }

  // --------------------------
  // Open Dialogs
  // --------------------------
  openAddDialog() {
    // Reset fields
    this.addCourseCode = '';
    this.addCourseName = '';
    this.addContractNo = '';
    this.addContractDate = null;
    this.addStartDate = null;
    this.addContractor = '';
    this.addEndDate = null;
    this.addCost = '';
    this.addContent = '';
    this.addFileName = '';
    this.displayAddDialog = true;
  }

  openDetailDialog(contract: Contract) {
    this.detailContract = contract;
    this.displayDetailDialog = true;
  }

  openEditDialog(index: number) {
    this.editIndex = index;
    const contract = this.filteredContracts[index];

    // Convert string date -> Date object
    this.editContractDate = contract.contractDate ? this.stringToDate(contract.contractDate) : null;    
    this.editStartDate = contract.startDate ? this.stringToDate(contract.startDate) : null;
    this.editEndDate = contract.endDate ? this.stringToDate(contract.endDate) : null;

    this.editCourseCode = contract.courseCode;
    this.editCourseName = contract.courseName;
    this.editContractNo = contract.contractNo;
    this.editContractor = contract.contractor;
    this.editCost = contract.cost;
    this.editContent = contract.content;
    this.editFileName = contract.fileName || '';

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
  saveNewContract() {
    const contractDateString = this.addContractDate
      ? this.dateToString(this.addContractDate)
      : '';
    const startDateString = this.addStartDate
      ? this.dateToString(this.addStartDate)
      : '';
    const endDateString = this.addEndDate
      ? this.dateToString(this.addEndDate)
      : '';

    const newContract: Contract = {
      courseCode: this.addCourseCode,
      courseName: this.addCourseName,
      contractNo: this.addContractNo,
      contractDate: contractDateString,
      startDate: startDateString,
      contractor: this.addContractor,
      endDate: endDateString,
      cost: this.addCost,
      content: this.addContent,
      fileName: this.addFileName
    };

    this.contracts.push(newContract);
    this.onSearchChange(); // refresh filtered data
    this.displayAddDialog = false;
  }

  // --------------------------
  // Edit Dialog Save
  // --------------------------
  saveEditedContract() {
    if (this.editIndex < 0 || this.editIndex >= this.filteredContracts.length) {
      return;
    }

    const contractDateString = this.editContractDate
      ? this.dateToString(this.editContractDate)
      : '';
    const startDateString = this.editStartDate
      ? this.dateToString(this.editStartDate)
      : '';
    const endDateString = this.editEndDate
      ? this.dateToString(this.editEndDate)
      : '';

    // Update the original array item
    const globalIndex = this.contracts.indexOf(this.filteredContracts[this.editIndex]);
    this.contracts[globalIndex] = {
      courseCode: this.editCourseCode,
      courseName: this.editCourseName,
      contractNo: this.editContractNo,
      contractDate: contractDateString,
      startDate: startDateString,
      contractor: this.editContractor,
      endDate: endDateString,
      cost: this.editCost,
      content: this.editContent,
      fileName: this.editFileName
    };

    this.onSearchChange();
    this.displayEditDialog = false;
  }

  // --------------------------
  // Delete
  // --------------------------
  deleteContract(index: number) {
    const globalIndex = this.contracts.indexOf(this.filteredContracts[index]);
    this.contracts.splice(globalIndex, 1);
    this.onSearchChange();
  }

  // --------------------------
  // File Upload Handlers
  // --------------------------
  onAddFileSelect(event: any) {
    // For demonstration, we just take the file name
    const file = event.files[0];
    this.addFileName = file.name;
  }

  onEditFileSelect(event: any) {
    const file = event.files[0];
    this.editFileName = file.name;
  }

  downloadFile(fileName: string) {
    alert(`Tải xuống: ${fileName}`);
  }

  // --------------------------
  // Helper Functions
  // --------------------------
  private dateToString(date: Date): string {
    const yyyy = date.getFullYear();
    const mm = (date.getMonth() + 1).toString().padStart(2, '0');
    const dd = date.getDate().toString().padStart(2, '0');
    return `${yyyy}-${mm}-${dd}`;
  }

  private stringToDate(str: string): Date {
    const [year, month, day] = str.split('-').map(Number);
    return new Date(year, month - 1, day);
  }
}
