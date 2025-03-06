import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';

interface MenuItem {
  label: string;
  icon: string;
  link: string;
}

@Component({
  selector: 'app-syllabus-detail',
  templateUrl: './syllabus-detail.component.html',
  styleUrl: './syllabus-detail.component.scss',
    standalone: true,
    imports: [CommonModule, ButtonModule,RouterLink,TableModule,CardModule]
})

export class SyllabusDetailComponent {
  syllabusDetail = {
    documentType: 'Syllabus',
    program: 'Information Technology',
    decisionNo: '1234/QĐ-ĐHQG',
    courseName: 'Lập trình web nâng cao',
    courseNameEnglish: 'Advanced Web Programming',
    courseCode: 'IT4409',
    learningMethod: 'Blended Learning',
    credits: 3,
    degreeLevel: 'Bachelor',
    timeAllocation: '45 hours',
    prerequisite: 'Web Programming',
    description:
      'Advanced concepts in web development including frontend frameworks, backend APIs, and deployment',
    studentTask: 'Complete assignments, projects and attend lectures',
    tools: 'VS Code, Git, Node.js',
    note: 'Laptop required for all sessions',
    minGPA: 5.0,
    scoringScale: '0-10',
    approvedDate: '2024-01-15',
  };

  menuItems: MenuItem[] = [
    {
      label: 'Xem lịch sử thay đổi',
      icon: 'pi pi-history',
      link: '/curriculum/history'
    }
  ];

  materials = [
    { 
      no: 1, 
      description: 'Web Development Guide', 
      purpose: 'Main textbook',
      isbn: '978-0136151323',
      type: 'Book',
      note: 'Required',
      author: 'John Smith',
      publisher: 'Pearson',
      publishedDate: '2023',
      edition: '3rd'
    }
  ];

  // Course Learning Outcomes data
  clos = [
    {
      no: 1,
      name: 'Web Architecture',
      description: 'Understand and implement modern web architectures'
    }
  ];

  // Constructivist Questions data
  constructivistQuestions = [
    {
      no: 1,
      sessionNo: 1,
      name: 'Web Architecture',
      detail: 'How do modern web frameworks improve development efficiency?'
    }
  ];

  // Schedule data
  scheduleData = [
    {
      session: '',
      learningTeachingMethod: '',
      content: '',
      clo: '',
      itu: '',
      studentsMaterials: '',
      studentsTask: '',
      lecturersMaterials: '',
      lecturersTask: '',
      studentsLink: '',
      lecturersLink: ''
    }
  ];

}
