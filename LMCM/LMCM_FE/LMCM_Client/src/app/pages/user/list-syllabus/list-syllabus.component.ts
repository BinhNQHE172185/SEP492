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
import { MessageService } from 'primeng/api';
import { RouterLink, RouterModule } from '@angular/router';
@Component({
    selector: 'app-list-syllabus',
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
      ConfirmDialogModule,
      RouterLink,
      RouterModule
    ],
    providers: [ConfirmationService, MessageService],
    templateUrl: './list-syllabus.component.html',
    styleUrls: ['./list-syllabus.component.scss']
  })
  export class ListSyllabusComponent {
    searchText: string = '';
    currentPage: number = 1;
    itemsPerPage: number = 4;
  
    syllabusList = [
      { syllabusId: 'SYL001', subjectCode: 'MATH101', subjectName: 'Mathematics', syllabusName: 'Advanced Mathematics Syllabus', isActive: true, isApproved: true, decisionNo: '123/QD 03/15/2024' },
      { syllabusId: 'SYL002', subjectCode: 'PHY102', subjectName: 'Physics', syllabusName: 'General Physics Syllabus', isActive: true, isApproved: false, decisionNo: '124/QD 03/14/2024' },
      { syllabusId: 'SYL003', subjectCode: 'CHEM103', subjectName: 'Chemistry', syllabusName: 'Basic Chemistry Syllabus', isActive: false, isApproved: false, decisionNo: '125/QD 03/13/2024' }
    ];
  
    constructor(private confirmationService: ConfirmationService, private messageService: MessageService) {}
  
    filteredSyllabus() {
      return this.syllabusList
        .filter(p => p.subjectName.toLowerCase().includes(this.searchText.toLowerCase()))
        .slice((this.currentPage - 1) * this.itemsPerPage, this.currentPage * this.itemsPerPage);
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
  
    deleteItem(syllabusId: string) {
      this.syllabusList = this.syllabusList.filter(item => item.syllabusId !== syllabusId);
    }
  

    onSearchChange(event: Event) {
        const inputElement = event.target as HTMLInputElement;
        this.searchText = inputElement.value;
      }
    
      totalPages() {
        return Array.from({ length: Math.ceil(this.syllabusList.length / this.itemsPerPage) }, (_, i) => i + 1);
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
  }
  