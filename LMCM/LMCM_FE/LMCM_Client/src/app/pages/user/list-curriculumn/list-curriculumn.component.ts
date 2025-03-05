import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { InputGroupModule } from 'primeng/inputgroup';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-list-curriculumn',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    CardModule,
    TagModule,
    InputGroupModule,
    ConfirmDialogModule
  ],
  templateUrl: './list-curriculumn.component.html',
  styleUrls: ['./list-curriculumn.component.scss'],
  providers: [ConfirmationService] // Cần để sử dụng ConfirmDialog
})
export class ListCurriculumnComponent {
  searchText: string = '';
  currentPage: number = 1;
  itemsPerPage: number = 4;

  programs = [
    { code: 'CUR001', name: 'Software Engineering', description: 'Core principles of software development and architecture', decision: 'QD-2023-001', approvalDate: '2023-01-15' },
    { code: 'CUR002', name: 'Data Science', description: 'Statistical analysis and machine learning fundamentals', decision: 'QD-2023-002', approvalDate: '2023-02-20' },
    { code: 'CUR003', name: 'Web Development', description: 'Frontend and backend web technologies', decision: 'QD-2023-003', approvalDate: '2023-03-10' },
    { code: 'CUR004', name: 'Mobile Development', description: 'iOS and Android application development', decision: 'QD-2023-004', approvalDate: '2023-04-05' }
  ];

  constructor(private confirmationService: ConfirmationService) {}

  filteredPrograms() {
    return this.programs
      .filter(p => p.name.toLowerCase().includes(this.searchText.toLowerCase()))
      .slice((this.currentPage - 1) * this.itemsPerPage, this.currentPage * this.itemsPerPage);
  }

  onSearchChange(event: Event) {
    const inputElement = event.target as HTMLInputElement;
    this.searchText = inputElement.value;
  }

  totalPages() {
    return Array.from({ length: Math.ceil(this.programs.length / this.itemsPerPage) }, (_, i) => i + 1);
  }

  setPage(page: number) {
    this.currentPage = page;
  }

  prevPage() {
    if (this.currentPage > 1) this.currentPage--;
  }

  nextPage() {
    if (this.currentPage < this.totalPages().length) this.currentPage++;
  }

  confirmDelete(code: string) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa chương trình này không?',
      header: 'Xác nhận xóa',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Xóa',
      rejectLabel: 'Hủy',
      accept: () => {
        this.deleteItem(code);
      }
    });
  }

  deleteItem(code: string) {
    this.programs = this.programs.filter(p => p.code !== code);
  }
}
