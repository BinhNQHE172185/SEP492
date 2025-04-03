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
import { LearningMaterialComponent } from './components/learning-material/learning-material.component';
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
    TextareaModule,
    LearningMaterialComponent
  ],
  providers: [ConfirmationService, MessageService]
})
export class SyllabusDetailComponent implements OnInit {
  syllabusId: string = '';
  syllabusDetail!: any;
  materials!: any;
  materialsDetail!: any;
  SelectedId: string | null = null;

  displayDetailDialog: boolean = false;
  displayAddDialog: boolean = false;

  isDetail: boolean = true;

  constructor(
    private syllabusService: SyllabusApiService,
    private materialService: LearningMaterialApiService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private router: Router,
    private route: ActivatedRoute,
    private fb: FormBuilder,
  ) {
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

  getLearningMaterial() {
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

  getLearningMaterialById() {
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
  selectedMaterial: any = null;
  deleteDialogVisible: boolean = false;
  materialForm!: FormGroup;
  isEdit: boolean = false;
  currentMaterialId: number | null = null;
  authors: string[] = [];

  openDetailDialog(id: string) {
    this.SelectedId = id;
    this.displayDetailDialog = true;
  }

  handleCloseDialog(isDetail: boolean) {
    if (isDetail) {
      this.displayDetailDialog = false;
    } else {
      this.displayAddDialog = false;
      this.SelectedId = null;
      this.getLearningMaterial();
    }
  }

  openAddDialog(id?: string) {
    if (id) {
      this.SelectedId = id;
    }
    else {
      this.SelectedId = null;
    }
    this.displayAddDialog = true;
  }

  confirmDeleteMaterial(id: Material): void {
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn muốn xóa học liệu này không?`,
      header: 'Xác nhận xóa',
      accept: () => {
        this.deleteMaterial(id);
      }
    });
  }

  deleteMaterial(id: any): void {
    this.materialService.deleteLearningMaterial(id).subscribe(
      (response) => {
        this.getLearningMaterial();
        this.messageService.add({ severity: 'success', summary: 'Thành công', detail: response.message });
        this.cancelDialog();
      },
      (error) => {
        this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: error.error.message });
      }
    );
    this.currentMaterialId = null;
  }

  cancelDialog(): void {
    this.materialDialog = false;
  }
}