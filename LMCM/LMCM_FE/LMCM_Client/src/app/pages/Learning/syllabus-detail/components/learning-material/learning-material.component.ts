import { Component, EventEmitter, Input, OnChanges, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { TabsModule } from 'primeng/tabs';
import { TextareaModule } from 'primeng/textarea';
import { SelectModule } from 'primeng/select';
import { MaterialCategory, MaterialTypeConstant, Publisher } from '../../../../../../shared/Constants/TypeConstants';
import { DatePickerModule } from 'primeng/datepicker';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { LearningMaterialApiService } from '../../../../../apis/learning-materialAPIs/learning-material-api.service';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-learning-material',
  standalone: true,
  imports: [CommonModule,
    ReactiveFormsModule,
    DropdownModule,
    InputTextModule,
    TabsModule,
    TextareaModule,
    SelectModule,
    FormsModule,
    DatePickerModule,
    DialogModule,
    ButtonModule
  ],

  templateUrl: './learning-material.component.html',
  styleUrls: ['./learning-material.component.scss'],
  providers: [ConfirmationService, MessageService]
})
export class LearningMaterialComponent implements OnChanges {
  @Input() SelectedId: any;
  @Input() displayAddDialog: boolean = false;
  @Output() closeDialogEvent = new EventEmitter<void>();

  materialForm!: FormGroup;
  materialDetailForm!: FormGroup;

  type = [
    { name: 'Online', value: MaterialTypeConstant.Online },
    { name: 'Offline', value: MaterialTypeConstant.Offline }
  ];

  constructor(private fb: FormBuilder,
    private learningMaterialApiService: LearningMaterialApiService,
    private messageService: MessageService,
    private route: ActivatedRoute,
  ) { }

  materialTypes = [
    { name: 'Sách xuất bản chính thức', value: MaterialCategory.PublishedBook },
    { name: 'Sách xuất bản nội bộ', value: MaterialCategory.InternalBook },
    { name: 'e-book có bản quyền', value: MaterialCategory.LicensedEbook },
    { name: 'Sách nguồn mở', value: MaterialCategory.OpenSourceBook },
    { name: 'Tài nguyên trên mạng (website, youtube, ...)', value: MaterialCategory.OnlineResource },
    { name: 'Khóa học Udemy', value: MaterialCategory.UdemyCourse }
  ];

  publishers = Object.values(Publisher).map(name => ({ name, value: name }));

  ngOnChanges() {
    this.resetForm();
    if (this.SelectedId) {
      this.learningMaterialApiService.getLearningMaterialDetail(this.SelectedId).subscribe(
        (response) => {
          if (response) {
            this.materialForm.patchValue(response);
          }
        },
        (error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Thất bại',
            detail: error?.error?.message || 'Đã xảy ra lỗi khi tải dữ liệu.'
          });
        }
      );
    } else {
    }
  }

  save() {
    if (this.SelectedId) {
      // this.learningMaterialApiService.updateLearningMaterial(this.SelectedId, this.materialForm.value).subscribe(
      //   (response) => {
      //     this.messageService.add({ severity: 'success', summary: 'Thành công', detail: response.message });
      //     this.closeDialog();
      //   },
      //   (error) => {
      //     this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: error.error.message });
      //   }
      // );
    } else {
      this.materialForm.patchValue({ syllabusId: this.route.snapshot.paramMap.get('id') || '' });
      if(!this.materialForm.get('materialDetail')?.value) {
        this.learningMaterialApiService.createLearningMaterial(this.materialForm.value).subscribe(
          (response) => {
            this.messageService.add({ severity: 'success', summary: 'Thành công', detail: response.message });
            this.closeDialog();
          },
          (error) => {
            this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: error.error.message });
          }
        );
      }else{
        this.learningMaterialApiService.createLearningMaterial(this.materialDetailForm.value).subscribe(
          (response) => {
            this.messageService.add({ severity: 'success', summary: 'Thành công', detail: response.message });
            this.closeDialog();
          },
          (error) => {
            this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: error.error.message });
          }
        );
      }
    }
  }

  resetForm() {
    this.materialForm = this.fb.group({
      syllabusId: [''],
      materialType: ['', Validators.required],
      materialQuantity: [''],
      purpose: [''],
      learningType: [''],
      note: [''],
      url: [''],
      materialDetail: null
    });

    this.materialDetailForm = this.fb.group({
      syllabusId: [''],
      materialName: ['', Validators.required],
      materialDescription: [''], 
      type: [null],
      publishedDate: [null], 
      publisher: [null], 
      edition: [''], 
      isbn: [''],
      note: ['']
    });
    
  }

  closeDialog() {
    this.resetForm();
    this.displayAddDialog = false;
    this.closeDialogEvent.emit();
  }
}
