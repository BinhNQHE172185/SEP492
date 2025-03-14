import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { TextareaModule } from 'primeng/textarea';

interface Material {
  id: number;
  no: number;
  description: string;
  purpose: string;
  isbn: string;
  type: string;
  note: string;
  author: string;
  publisher: string;
  publishedDate: string;
  edition: string;
}

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
  imports: [
    CommonModule, 
    ButtonModule,
    RouterLink,
    TableModule,
    CardModule,
    DialogModule,
    InputTextModule,
    TextareaModule,
    InputNumberModule,
    CalendarModule,
    DropdownModule,
    FormsModule,
    ReactiveFormsModule,
    ConfirmDialogModule,
    ToastModule
  ],
  providers: [ConfirmationService, MessageService]
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
  
  materials: Material[] = [
    {
      id: 1,
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

  // Material dialog properties
  materialDialog: boolean = false;
  deleteDialogVisible: boolean = false;
  materialForm!: FormGroup;
  isEdit: boolean = false;
  currentMaterialId: number | null = null;
  
  // Material type options
  materialTypes = [
    { label: 'Book', value: 'Book' },
    { label: 'Article', value: 'Article' },
    { label: 'Website', value: 'Website' },
    { label: 'Video', value: 'Video' },
    { label: 'Software', value: 'Software' },
    { label: 'Other', value: 'Other' }
  ];

  constructor(
    private fb: FormBuilder,
    private confirmationService: ConfirmationService,
    private messageService: MessageService
  ) {
    this.initForm();
  }

  initForm(): void {
    this.materialForm = this.fb.group({
      description: ['', Validators.required],
      purpose: ['', Validators.required],
      isbn: [''],
      type: ['Book', Validators.required],
      note: [''],
      author: ['', Validators.required],
      publisher: [''],
      publishedDate: [''],
      edition: ['']
    });
  }

  openNewMaterialDialog(): void {
    this.isEdit = false;
    this.currentMaterialId = null;
    this.materialForm.reset({type: 'Book'});
    this.materialDialog = true;
  }

  openEditMaterialDialog(material: Material): void {
    this.isEdit = true;
    this.currentMaterialId = material.id;
    this.materialForm.patchValue({
      description: material.description,
      purpose: material.purpose,
      isbn: material.isbn,
      type: material.type,
      note: material.note,
      author: material.author,
      publisher: material.publisher,
      publishedDate: material.publishedDate,
      edition: material.edition
    });
    this.materialDialog = true;
  }

  confirmDeleteMaterial(material: Material): void {
    this.currentMaterialId = material.id;
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn muốn xóa tài liệu "${material.description}"?`,
      header: 'Xác nhận xóa',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.deleteMaterial();
      }
    });
  }

  deleteMaterial(): void {
    this.materials = this.materials.filter(m => m.id !== this.currentMaterialId);
    // Renumber remaining materials
    this.materials = this.materials.map((m, index) => {
      return {...m, no: index + 1};
    });
    this.messageService.add({severity:'success', summary: 'Thành công', detail: 'Đã xóa tài liệu', life: 3000});
    this.currentMaterialId = null;
  }

  saveMaterial(): void {
    if (this.materialForm.invalid) {
      Object.keys(this.materialForm.controls).forEach(key => {
        const control = this.materialForm.get(key);
        if (control) {
          control.markAsDirty();
          control.markAsTouched();
        }
      });
      return;
    }
    
    const formValues = this.materialForm.value;
    
    if (this.isEdit && this.currentMaterialId) {
      // Update existing material
      const index = this.materials.findIndex(m => m.id === this.currentMaterialId);
      if (index !== -1) {
        this.materials[index] = {
          ...this.materials[index],
          description: formValues.description,
          purpose: formValues.purpose,
          isbn: formValues.isbn,
          type: formValues.type,
          note: formValues.note,
          author: formValues.author,
          publisher: formValues.publisher,
          publishedDate: formValues.publishedDate,
          edition: formValues.edition
        };
        this.messageService.add({severity:'success', summary: 'Thành công', detail: 'Đã cập nhật tài liệu', life: 3000});
      }
    } else {
      // Add new material
      const newId = this.materials.length > 0 ? Math.max(...this.materials.map(m => m.id)) + 1 : 1;
      const newNo = this.materials.length + 1;
      
      this.materials.push({
        id: newId,
        no: newNo,
        description: formValues.description,
        purpose: formValues.purpose,
        isbn: formValues.isbn || '',
        type: formValues.type,
        note: formValues.note || '',
        author: formValues.author,
        publisher: formValues.publisher || '',
        publishedDate: formValues.publishedDate || '',
        edition: formValues.edition || ''
      });
      this.messageService.add({severity:'success', summary: 'Thành công', detail: 'Đã thêm tài liệu mới', life: 3000});
    }
    
    this.materialDialog = false;
    this.materialForm.reset({type: 'Book'});
  }

  cancelDialog(): void {
    this.materialDialog = false;
    this.materialForm.reset({type: 'Book'});
  }
}