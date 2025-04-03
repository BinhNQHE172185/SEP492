import { Component, EventEmitter, Input, OnChanges, Output } from '@angular/core';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
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
import { CheckboxModule } from 'primeng/checkbox';
import { ToastModule } from 'primeng/toast';
import { ChipModule } from 'primeng/chip';
import { AutoCompleteCompleteEvent, AutoCompleteModule } from 'primeng/autocomplete';

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
    ButtonModule,
    CheckboxModule,
    ToastModule,
    AutoCompleteModule
  ],

  templateUrl: './learning-material.component.html',
  styleUrls: ['./learning-material.component.scss'],
  providers: [ConfirmationService, MessageService]
})
export class LearningMaterialComponent implements OnChanges {
  @Input() SelectedId: any;
  @Input() displayAddDialog: boolean = false;
  @Output() closeDialogEvent = new EventEmitter<void>();

  isImported: boolean = false;
  items: any[] = [];
  authors: string[] = [];
  publishersList: string[] = [];
  filteredPublishersList: string[] = [];
  material: any;

  type = [
    { name: 'Online', value: MaterialTypeConstant.Online },
    { name: 'Offline', value: MaterialTypeConstant.Offline }
  ];

  constructor(
    private learningMaterialApiService: LearningMaterialApiService,
    private messageService: MessageService,
    private route: ActivatedRoute,
  ) {

  }

  materialTypes = [
    { name: 'Sách xuất bản chính thức', value: MaterialCategory.PublishedBook },
    { name: 'Sách xuất bản nội bộ', value: MaterialCategory.InternalBook },
    { name: 'e-book có bản quyền', value: MaterialCategory.LicensedEbook },
    { name: 'Sách nguồn mở', value: MaterialCategory.OpenSourceBook },
    { name: 'Tài nguyên trên mạng (website, youtube, ...)', value: MaterialCategory.OnlineResource },
    { name: 'Khóa học Udemy', value: MaterialCategory.UdemyCourse }
  ];

  ngOnChanges() {
    this.resetForm();
    if (this.SelectedId) {
      this.learningMaterialApiService.getLearningMaterialDetail(this.SelectedId).subscribe(
        (response) => {
          if (response) {
            this.material = response;
            this.material.publishedDate = new Date(response.publishedDate);
            this.authors = response.author ? response.author.split(',').map((author: string) => author.trim()) : [];
            this.isImported = response.isImportedMaterial;
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
    }
    this.learningMaterialApiService.getPublishers().subscribe(
      (response) => {
        if (response) {
          this.publishersList = response;
          this.filteredPublishersList = [...this.publishersList];
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
  }

  save() {
    this.material.author = this.authors.join(', ');
    if (this.SelectedId) {
      this.learningMaterialApiService.updateLearningMaterial(this.SelectedId, this.material).subscribe(
        (response) => {
          this.messageService.add({ severity: 'success', summary: 'Thành công', detail: response.message });
          this.closeDialog();
        },
        (error) => {
          this.messageService.add({ severity: 'error', summary: 'Thất bại', detail: error.error.message });
        }
      );
    } else {
      this.material.syllabusId = this.route.snapshot.paramMap.get('id') || '';
      this.learningMaterialApiService.createLearningMaterial(this.material).subscribe(
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

  search(event: AutoCompleteCompleteEvent) {
    this.items = [...Array(1).keys()].map(() => event.query);
  }

  searchPublisher(event: AutoCompleteCompleteEvent) {
    const query = event.query.toLowerCase();
    if (query === '') {
      this.filteredPublishersList = [...this.publishersList];
    } else {
      this.filteredPublishersList = this.publishersList.filter(publisher =>
        publisher.toLowerCase().includes(query)
      );
    }

    console.log(this.filteredPublishersList);
    console.log(query);
  }

  resetForm(): void {
    this.isImported = false;
    this.material = {
      syllabusId: '',
      learningType: '',
      materialType: '',
      isMainMaterial: false,
      materialName: '',
      isbn: '',
      publisher: '',
      publishedDate: null,
      edition: '',
      url: '',
      purpose: '',
      note: '',
      author: ''
    };
  }

  closeDialog() {
    this.resetForm();
    this.displayAddDialog = false;
    this.closeDialogEvent.emit();
  }
}
