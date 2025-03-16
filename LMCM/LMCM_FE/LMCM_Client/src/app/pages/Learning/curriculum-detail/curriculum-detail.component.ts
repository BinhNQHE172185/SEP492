import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { RouterLink } from '@angular/router';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { IconFieldModule } from 'primeng/iconfield';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { TagModule } from 'primeng/tag';
import { PanelModule } from 'primeng/panel';


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
  imports: [
    CommonModule,
    ButtonModule,
    RouterLink,
    CardModule,
    TableModule,
    IconFieldModule,
    InputTextModule,
    TextareaModule,
    TagModule,
    PanelModule
  ]
})

export class CurriculumDetailComponent {
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
        { code: 'MATH102', name: 'Calculus 2', nameVn: 'Giải tích 2', credits: 3, hours: 45 },
        { code: 'COMP102', name: 'Object-Oriented Programming', nameVn: 'Lập trình hướng đối tượng', credits: 4, hours: 60 },
        { code: 'ENG102', name: 'English 2', nameVn: 'Tiếng Anh 2', credits: 3, hours: 45 },
        { code: 'ELEC101', name: 'Electrical Engineering', nameVn: 'Kỹ thuật điện', credits: 3, hours: 45 },
        { code: 'STAT101', name: 'Statistics', nameVn: 'Thống kê', credits: 3, hours: 45 }
      ]
    },
    {
      name: 'Semester',
      number: 3,
      subjectCount: 5,
      subjects: [
        { code: 'COMP201', name: 'Data Structures & Algorithms', nameVn: 'Cấu trúc dữ liệu & Giải thuật', credits: 4, hours: 60 },
        { code: 'DB101', name: 'Database Fundamentals', nameVn: 'Cơ sở dữ liệu', credits: 3, hours: 45 },
        { code: 'NET101', name: 'Computer Networks', nameVn: 'Mạng máy tính', credits: 3, hours: 45 },
        { code: 'OS101', name: 'Operating Systems', nameVn: 'Hệ điều hành', credits: 3, hours: 45 },
        { code: 'AI101', name: 'Introduction to AI', nameVn: 'Nhập môn trí tuệ nhân tạo', credits: 3, hours: 45 }
      ]
    },
    {
      name: 'Semester',
      number: 4,
      subjectCount: 5,
      subjects: [
        { code: 'WEB101', name: 'Web Development', nameVn: 'Phát triển Web', credits: 4, hours: 60 },
        { code: 'MOBILE101', name: 'Mobile Development', nameVn: 'Lập trình di động', credits: 4, hours: 60 },
        { code: 'CLOUD101', name: 'Cloud Computing', nameVn: 'Điện toán đám mây', credits: 3, hours: 45 },
        { code: 'CYBER101', name: 'Cyber Security', nameVn: 'An toàn thông tin', credits: 3, hours: 45 },
        { code: 'SOFTENG101', name: 'Software Engineering', nameVn: 'Công nghệ phần mềm', credits: 3, hours: 45 }
      ]
    },
    {
      name: 'Semester',
      number: 5,
      subjectCount: 5,
      subjects: [
        { code: 'ML101', name: 'Machine Learning', nameVn: 'Học máy', credits: 3, hours: 45 },
        { code: 'BIGDATA101', name: 'Big Data Analytics', nameVn: 'Phân tích dữ liệu lớn', credits: 3, hours: 45 },
        { code: 'DEVOPS101', name: 'DevOps', nameVn: 'DevOps', credits: 3, hours: 45 },
        { code: 'IOT101', name: 'Internet of Things', nameVn: 'Internet vạn vật', credits: 3, hours: 45 },
        { code: 'UX101', name: 'UX/UI Design', nameVn: 'Thiết kế UX/UI', credits: 3, hours: 45 }
      ]
    },
    {
      name: 'Semester',
      number: 6,
      subjectCount: 5,
      subjects: [
        { code: 'ROBOT101', name: 'Robotics', nameVn: 'Ngành robot', credits: 3, hours: 45 },
        { code: 'BLOCK101', name: 'Blockchain Technology', nameVn: 'Công nghệ Blockchain', credits: 3, hours: 45 },
        { code: 'GAME101', name: 'Game Development', nameVn: 'Lập trình Game', credits: 4, hours: 60 },
        { code: 'VR101', name: 'Virtual Reality', nameVn: 'Thực tế ảo', credits: 3, hours: 45 },
        { code: 'ENTREP101', name: 'Entrepreneurship in IT', nameVn: 'Khởi nghiệp trong CNTT', credits: 3, hours: 45 }
      ]
    },
    {
      name: 'Semester',
      number: 7,
      subjectCount: 5,
      subjects: [
        { code: 'ADVAI101', name: 'Advanced AI', nameVn: 'Trí tuệ nhân tạo nâng cao', credits: 3, hours: 45 },
        { code: 'SEC101', name: 'Advanced Cybersecurity', nameVn: 'An toàn thông tin nâng cao', credits: 3, hours: 45 },
        { code: 'MATH201', name: 'Mathematics for CS', nameVn: 'Toán cho khoa học máy tính', credits: 3, hours: 45 },
        { code: 'LAW101', name: 'IT Law & Ethics', nameVn: 'Luật CNTT & Đạo đức', credits: 3, hours: 45 },
        { code: 'BIZ101', name: 'Business IT', nameVn: 'CNTT trong kinh doanh', credits: 3, hours: 45 }
      ]
    },
    {
      name: 'Semester',
      number: 8,
      subjectCount: 5,
      subjects: [
        { code: 'PROJECT101', name: 'Capstone Project', nameVn: 'Đồ án tốt nghiệp', credits: 6, hours: 90 },
        { code: 'RES101', name: 'Research Methods', nameVn: 'Phương pháp nghiên cứu', credits: 3, hours: 45 },
        { code: 'STARTUP101', name: 'IT Startup', nameVn: 'Khởi nghiệp CNTT', credits: 3, hours: 45 },
        { code: 'INDUST101', name: 'Industry Internship', nameVn: 'Thực tập ngành', credits: 6, hours: 90 },
        { code: 'PROFDEV101', name: 'Professional Development', nameVn: 'Phát triển nghề nghiệp', credits: 3, hours: 45 }
      ]
    },
    {
      name: 'Semester',
      number: 9,
      subjectCount: 5,
      subjects: [
        { code: 'THESIS101', name: 'Graduation Thesis', nameVn: 'Luận văn tốt nghiệp', credits: 9, hours: 135 },
        { code: 'MANAGE101', name: 'IT Project Management', nameVn: 'Quản lý dự án CNTT', credits: 3, hours: 45 },
        { code: 'INNOV101', name: 'Innovation & Creativity', nameVn: 'Sáng tạo & đổi mới', credits: 3, hours: 45 },
        { code: 'CONSULT101', name: 'IT Consulting', nameVn: 'Tư vấn CNTT', credits: 3, hours: 45 },
        { code: 'TECHFUTURE101', name: 'Future Technology Trends', nameVn: 'Xu hướng công nghệ', credits: 3, hours: 45 }
      ]
    }
  ];
}