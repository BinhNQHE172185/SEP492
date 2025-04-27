import { Component } from '@angular/core';
import { ContractValueApiComponent } from '../../../apis/contractValueAPIs/contract-value-api.servcie';
import { TableModule } from 'primeng/table';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { TagModule } from 'primeng/tag';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';

@Component({
  selector: 'app-contract-value',
  imports: [
    TableModule,
    CardModule,
    ButtonModule,
    ToastModule,
    TagModule,
    ProgressSpinnerModule,
    CommonModule,
    FormsModule,
    InputNumberModule,
    InputTextModule,
    TextareaModule
  ],
  standalone: true,
  templateUrl: './contract-value.component.html',
  styleUrl: './contract-value.component.scss',
  providers: [
    MessageService,
  ]
})
export class ContractValueComponent {
  contractValues: any[] = [];
  isLoading: boolean = false;

  constructor(
    private contractValueService: ContractValueApiComponent,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.loadData();
  }
  loadData() {
    this.contractValueService.getContractValue().subscribe(
      (response) => {
        this.contractValues = response;
      },
      (error) => {
        this.messageService.add({
          severity: 'error',

        }
        )
      });
  }


  updateContractValue() {
    const request = this.contractValues.map(item => ({
      valueId: item.valueId,
      category: item.category,
      measurementUnit: item.measurementUnit,
      standardRate: item.standardRate,
      qualityRequirements: item.qualityRequirements,
      contractValue: item.contractValue,
      valueRatioForUpdate: item.valueRatioForUpdate
    }));
    this.isLoading = true;
    this.contractValueService.updateContractValue(request).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.loadData();
        this.messageService.add({
          severity: 'success',
          summary: 'Thành công',
          detail: 'Cập nhật thành công',
        })
      },
      error: (err) => {
        this.isLoading = false;
        this.messageService.add({
          severity: 'error',
          summary: 'Thất bại',
          detail: err.error.message,
        })
      }
    });
  }

}
