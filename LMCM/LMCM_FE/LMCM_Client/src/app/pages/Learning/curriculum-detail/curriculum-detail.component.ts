import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { RouterLink } from '@angular/router';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { IconFieldModule } from 'primeng/iconfield';


interface CurriculumDetail {
  code: string;
  name: string;
  engName: string;
  engCurriculumName: string;
  majorCode: string;
  majorName: string;
  decisionNo: string;
  approvalDate: string;
  description: string;
}

interface Semester {
  name: string;
  number: number;
  subjectCount: number;
  subjects: Subject[];
}

interface Subject {
  code: string;
  name: string;
  nameVn?: string;
  credits: number;
  hours: number;
}



@Component({
  selector: 'app-curriculum-detail',
  templateUrl: './curriculum-detail.component.html',
  styleUrls: ['./curriculum-detail.component.scss'],
  standalone: true,
  imports: [CommonModule, ButtonModule,RouterLink,CardModule,TableModule,IconFieldModule]
})

export class CurriculumDetailComponent   {
  curriculumDetail: CurriculumDetail = {
    code: 'CUR-2024-001',
    name: 'Chương Trình Công Nghệ Thông Tin',
    engName: 'Information Technology',
    engCurriculumName: 'Information Technology Program',
    majorCode: 'VOC-IT-001',
    majorName: 'Công Nghệ Thông Tin',
    decisionNo: '1234/QĐ-ĐHQG',
    approvalDate: '2024-01-15',
    description: 'Chương trình đào tạo được thiết kế nhằm cung cấp kiến thức và kỹ năng toàn diện về công nghệ thông tin, bao gồm lập trình, cơ sở dữ liệu, mạng máy tính và an ninh mạng.'
  };


  semesters: Semester[] = [
    {
      name: 'Semester',
      number: 1,
      subjectCount: 5,
      subjects: [
        { code: 'MATH101', name: 'Calculus 1', nameVn: 'Giải tích 1', credits: 3, hours: 45 },
        { code: 'COMP101', name: 'Introduction to Programming', nameVn: 'Nhập môn lập trình', credits: 4, hours: 60 },
        { code: 'ENG101', name: 'English 1', nameVn: 'Tiếng Anh 1', credits: 3, hours: 45 },
        { code: 'PHY101', name: 'General Physics', nameVn: 'Vật lý đại cương', credits: 4, hours: 60 },
        { code: 'CHEM101', name: 'General Chemistry', nameVn: 'Hóa học đại cương', credits: 3, hours: 45 }
      ]
    },
    {
      name: 'Semester',
      number: 2,
      subjectCount: 5,
      subjects: [
        { code: 'SUB1', name: 'Subject 1', credits: 3, hours: 45 },
        { code: 'SUB2', name: 'Subject 2', credits: 3, hours: 45 },
        { code: 'SUB3', name: 'Subject 3', credits: 3, hours: 45 },
        { code: 'SUB4', name: 'Subject 4', credits: 3, hours: 45 },
        { code: 'SUB5', name: 'Subject 5', credits: 3, hours: 45 }
      ]
    },
    {
      name: 'Semester',
      number: 3,
      subjectCount: 5,
      subjects: [
        { code: 'SUB1', name: 'Subject 1', credits: 3, hours: 45 },
        { code: 'SUB2', name: 'Subject 2', credits: 3, hours: 45 },
        { code: 'SUB3', name: 'Subject 3', credits: 3, hours: 45 },
        { code: 'SUB4', name: 'Subject 4', credits: 3, hours: 45 },
        { code: 'SUB5', name: 'Subject 5', credits: 3, hours: 45 }
      ]
    },
    {
      name: 'Semester',
      number: 4,
      subjectCount: 5,
      subjects: [
        { code: 'SUB1', name: 'Subject 1', credits: 3, hours: 45 },
        { code: 'SUB2', name: 'Subject 2', credits: 3, hours: 45 },
        { code: 'SUB3', name: 'Subject 3', credits: 3, hours: 45 },
        { code: 'SUB4', name: 'Subject 4', credits: 3, hours: 45 },
        { code: 'SUB5', name: 'Subject 5', credits: 3, hours: 45 }
      ]
    },
    {
      name: 'Semester',
      number: 5,
      subjectCount: 5,
      subjects: [
        { code: 'SUB1', name: 'Subject 1', credits: 3, hours: 45 },
        { code: 'SUB2', name: 'Subject 2', credits: 3, hours: 45 },
        { code: 'SUB3', name: 'Subject 3', credits: 3, hours: 45 },
        { code: 'SUB4', name: 'Subject 4', credits: 3, hours: 45 },
        { code: 'SUB5', name: 'Subject 5', credits: 3, hours: 45 }
      ]
    }
  ];



}