import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
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
import { InputGroupModule } from 'primeng/inputgroup';
import { InputGroupAddonModule } from 'primeng/inputgroupaddon';
import { FieldsetModule } from 'primeng/fieldset';
import { DividerModule } from 'primeng/divider';
import { PanelModule } from 'primeng/panel';
import { SyllabusApiService } from '../../../apis/syllabusAPIs/syllabus-api.service';
import { LearningMaterialApiService } from '../../../apis/learning-materialAPIs/learning-material-api.service';
interface Material {
  id: number;
  no: number;
  description: string;

  isbn: string;
  type: string;

  author: string;
  publisher: string;
  publishedDate: string
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
    ToastModule,
    InputGroupModule,
    InputGroupAddonModule,
    FieldsetModule,
    DividerModule,
    PanelModule,
    TextareaModule
  ],
  providers: [ConfirmationService, MessageService]
})
export class SyllabusDetailComponent implements OnInit {
  syllabusId: string = '';
  syllabusDetail!: any;
  materials!: any;
  materialsDetail!: any;

  constructor(
    private syllabusService: SyllabusApiService,
    private materialService: LearningMaterialApiService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private router: Router,
    private route: ActivatedRoute,
    private fb: FormBuilder,
  ) {
    this.initForm();
  }

  ngOnInit() {
    this.syllabusId = this.route.snapshot.paramMap.get('id') || '';
    this.getDetail();
    this.getLearningMaterial();
  }

  getDetail() {
    const id = this.syllabusId;
    if (id) {
      this.syllabusService.getSyllabusDetail(id).subscribe({
        next: (data) => {
          this.syllabusDetail = data
        },
        error: (err) => {
          console.error('Error fetching curriculum detail:', err);
        }
      });
    }
  }

  getLearningMaterial(){
    const id = this.syllabusId;
    if (id) {
      this.materialService.getLearningMaterialList(id).subscribe({
        next: (data) => {
          this.materials = data
        },
        error: (err) => {
          console.error('Error fetching curriculum detail:', err);
        }
      });
    }
  }

  getLearningMaterialById(){
    const id = this.syllabusId;
    if (id) {
      this.materialService.getLearningMaterialById(id).subscribe({
        next: (data) => {
          this.materialsDetail = data
        },
        error: (err) => {
          console.error('Error fetching curriculum detail:', err);
        }
      });
    }
  }

  // Material dialog properties
  materialDialog: boolean = false;
  deleteDialogVisible: boolean = false;
  materialForm!: FormGroup;
  isEdit: boolean = false;
  currentMaterialId: number | null = null;
  authors: string[] = [];

  // Material type options
  materialTypes = [
    { label: 'Sách', value: 'Book' },
    { label: 'Bài báo', value: 'Article' },
    { label: 'Website', value: 'Website' },
    { label: 'Video', value: 'Video' },
    { label: 'Phần mềm', value: 'Software' },
    { label: 'Khác', value: 'Other' }
  ];

  // Publisher options (for dropdown)
  publishers = [
    { label: 'NXB Giáo dục', value: 'NXB Giáo dục' },
    { label: 'NXB Đại học Quốc gia', value: 'NXB Đại học Quốc gia' },
    { label: 'NXB Khoa học Kỹ thuật', value: 'NXB Khoa học Kỹ thuật' },
    { label: 'NXB Thông tin và Truyền thông', value: 'NXB Thông tin và Truyền thông' }
  ];



  initForm(): void {
    this.materialForm = this.fb.group({
      description: ['', Validators.required],
      publishedDate: [''],
      originalBook: [''],
      author: [''],
      publisher: [''],
      originalPublishedDate: [''],
      edition: [''],
      isbn: [''],
      type: ['Book', Validators.required]
    });
  }

  openNewMaterialDialog(): void {
    this.isEdit = false;
    this.currentMaterialId = null;
    this.materialForm.reset({ type: 'Book' });
    this.authors = [];
    this.materialDialog = true;
  }

  openEditMaterialDialog(material: Material): void {
    this.isEdit = true;
    this.currentMaterialId = material.id;

    // Parse and set dates properly
    let publishedDate = null;
    if (material.publishedDate) {
      try {
        publishedDate = new Date(material.publishedDate);
      } catch (e) {
        publishedDate = null;
      }
    }

    this.materialForm.patchValue({
      description: material.description,
      publishedDate: publishedDate,
      publisher: material.publisher,
      isbn: material.isbn,
      type: material.type
    });

    // Set authors array
    this.authors = material.author ? material.author.split(', ') : [];

    this.materialDialog = true;
  }

  confirmDeleteMaterial(material: Material): void {
    this.currentMaterialId = material.id;
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn muốn xóa học liệu "${material.description}"?`,
      header: 'Xác nhận xóa',
      accept: () => {
        this.deleteMaterial();
      }
    });
  }

  deleteMaterial(): void {
    this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Đã xóa học liệu', life: 3000 });
    this.currentMaterialId = null;
  }

  addAuthor(): void {
    const authorValue = this.materialForm.get('author')?.value;
    if (authorValue && authorValue.trim()) {
      // Check for duplicates
      if (!this.authors.includes(authorValue.trim())) {
        this.authors.push(authorValue.trim());
        this.materialForm.get('author')?.setValue('');
      } else {
        this.messageService.add({
          severity: 'warn',
          summary: 'Cảnh báo',
          detail: 'Tác giả này đã được thêm vào danh sách',
          life: 3000
        });
      }
    }
  }

  removeAuthor(index: number): void {
    this.authors.splice(index, 1);
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

    // Check if at least one author exists
    if (this.authors.length === 0) {
      this.messageService.add({
        severity: 'error',
        summary: 'Lỗi',
        detail: 'Vui lòng thêm ít nhất một tác giả',
        life: 3000
      });
      return;
    }

    const formValues = this.materialForm.value;
    const authorString = this.authors.join(', ');

    // if (this.isEdit && this.currentMaterialId) {
    //   // Update existing material
    //   const index = this.materials.findIndex(m => m.id === this.currentMaterialId);
    //   if (index !== -1) {
    //     this.materials[index] = {
    //       ...this.materials[index],
    //       description: formValues.description,
    //       publishedDate: formValues.publishedDate,
    //       author: authorString,
    //       publisher: formValues.publisher || '',
    //       isbn: formValues.isbn || '',
    //       type: formValues.type
    //     };
    //     this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Đã cập nhật học liệu', life: 3000 });
    //   }
    // } else {
    //   // Add new material
    //   const newId = this.materials.length > 0 ? Math.max(...this.materials.map(m => m.id)) + 1 : 1;
    //   const newNo = this.materials.length + 1;

    //   this.materials.push({
    //     id: newId,
    //     no: newNo,
    //     description: formValues.description,
    //     publishedDate: formValues.publishedDate,
    //     author: authorString,
    //     publisher: formValues.publisher || '',
    //     isbn: formValues.isbn || '',
    //     type: formValues.type
    //   });
    //   this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Đã thêm học liệu mới', life: 3000 });
    // }

    this.materialDialog = false;
    this.materialForm.reset({ type: 'Book' });
    this.authors = [];
  }

  cancelDialog(): void {
    this.materialDialog = false;
    this.materialForm.reset({ type: 'Book' });
    this.authors = [];
  }
}