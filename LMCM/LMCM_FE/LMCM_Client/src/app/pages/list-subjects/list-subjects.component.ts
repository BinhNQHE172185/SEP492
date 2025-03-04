import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { FormsModule } from '@angular/forms';
import { InputGroupModule } from 'primeng/inputgroup';

interface Subject {
  subjectCode: string;
  subjectName: string;
  englishName: string;
  previousCode?: string;
  isConstructivist: boolean;
  method: string;
  duration: string;
  reality: string;
}

@Component({
  standalone: true,
  imports: [
    InputGroupModule,FormsModule, CommonModule, TableModule, ButtonModule, TagModule, CardModule, InputTextModule
  ],
  selector: 'app-list-subjects',
  templateUrl: './list-subjects.component.html',
  styleUrls: ['./list-subjects.component.scss'],
})
export class ListSubjectsComponent {
  searchKeyword: string = '';
  subjects: Subject[] = [
    { subjectCode: 'MATH101', subjectName: 'Toán Cao Cấp', englishName: 'Advanced Mathematics', previousCode: '', isConstructivist: true, method: 'Traditional', duration: '90h', reality: '90h' },
    { subjectCode: 'PHY102', subjectName: 'Vật Lý Đại Cương', englishName: 'General Physics', previousCode: 'P100', isConstructivist: false, method: 'Traditional', duration: '30h', reality: '90h' },
    { subjectCode: 'CHEM103', subjectName: 'Hóa Học', englishName: 'Chemistry', previousCode: 'C100', isConstructivist: true, method: 'Blended', duration: '40h', reality: '90h' }
  ];

  get filteredSubjects(): Subject[] {
    if (!this.searchKeyword.trim()) {
      return this.subjects;
    }
    const keyword = this.searchKeyword.toLowerCase();
    return this.subjects.filter(subject =>
      subject.subjectCode.toLowerCase().includes(keyword) ||
      subject.subjectName.toLowerCase().includes(keyword) ||
      subject.englishName.toLowerCase().includes(keyword)
    );
  }

  getTagValue(value: boolean): 'Yes' | 'No' {
    return value ? 'Yes' : 'No';
  }

  deleteSubject(index: number) {
    this.subjects.splice(index, 1);
  }
}
