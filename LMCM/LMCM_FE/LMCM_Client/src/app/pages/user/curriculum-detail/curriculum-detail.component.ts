import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { RouterLink } from '@angular/router';

interface MenuItem {
  label: string;
  icon: string;
  link: string;
}

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

@Component({
  selector: 'app-curriculum-detail',
  templateUrl: './curriculum-detail.component.html',
  styleUrls: ['./curriculum-detail.component.scss'],
  standalone: true,
  imports: [CommonModule, ButtonModule,RouterLink]
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

  menuItems: MenuItem[] = [
    {
      label: 'Xem lịch sử thay đổi',
      icon: 'pi pi-history',
      link: '/curriculum/history'
    },
    {
      label: 'Xem PLO',
      icon: 'pi pi-file',
      link: '/user/plo'
    },
    {
      label: 'Xem Combo',
      icon: 'pi pi-th-large',
      link: '/curriculum/combo'
    },
    {
      label: 'Xem môn TC',
      icon: 'pi pi-list',
      link: '/curriculum/courses'
    }
  ];


}