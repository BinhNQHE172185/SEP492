import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { DropdownModule } from 'primeng/dropdown';
import { Calendar, CalendarModule } from 'primeng/calendar';
import { InputTextModule } from 'primeng/inputtext';
import { TabsModule } from 'primeng/tabs';
import { TextareaModule } from 'primeng/textarea';
import { SelectModule } from 'primeng/select';
import { MaterialTypeConstant } from '../../../../../../shared/Constants/TypeConstants';
import { DatePickerModule } from 'primeng/datepicker';

@Component({
  selector: 'app-learning-material',
  standalone: true,
  imports: [CommonModule,
    ReactiveFormsModule,
    DropdownModule,
    Calendar,
    InputTextModule,
    TabsModule,
    TextareaModule,
    SelectModule,
    FormsModule,
    DatePickerModule,
    CalendarModule
  ],

  templateUrl: './learning-material.component.html',
  styleUrls: ['./learning-material.component.scss'],
})
export class LearningMaterialComponent {
  @Input() selectedMaterial: any;
  @Output() closeDialog = new EventEmitter<void>();

  materialForm!: FormGroup;
  materialDetailForm!: FormGroup;

  type = [
    { name: 'Online', value: MaterialTypeConstant.Online },
    { name: 'Offline', value: MaterialTypeConstant.Offline }
  ];

  constructor(private fb: FormBuilder) { }

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

  ngOnInit() {
    this.materialForm = this.fb.group({
      description: ['', Validators.required],
      quantity: [''],
      purpuse: [''],
      isbn: [''],
      type: [''],
      note: [''],
      url: [''],
      selectedType: [''],
    });

    this.materialDetailForm = this.fb.group({
      materialName: ['', Validators.required], 
      type: [null, Validators.required],
      publishedDate: [null],
      originalBook: [''], 
      originalPublishedDate: [null], 
      publisher: [null],
      edition: [''], 
      isbn: ['']
    });
  }

  saveMaterial() {
    if (this.materialForm.valid) {
      console.log('Saved:', this.materialForm.value);
      this.closeDialog.emit();
    } else {
      this.materialForm.markAllAsTouched();
    }
  }

  closeMaterialDialog() {
    this.closeDialog.emit();
  }
}
