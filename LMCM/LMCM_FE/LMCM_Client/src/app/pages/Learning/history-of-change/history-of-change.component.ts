import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { ConfirmDialog } from 'primeng/confirmdialog';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { searchService } from '../../service/search/search-service.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-history-of-change',
  imports: [InputGroupModule, FormsModule, CommonModule, TableModule, ButtonModule, CardModule, InputTextModule, ConfirmDialog],
  templateUrl: './history-of-change.component.html',
  styleUrl: './history-of-change.component.scss'
})
export class HistoryOfChangeComponent {
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  searchKey = '';

  private searchSubscription!: Subscription;

    constructor( private searchService: searchService) { }
  

  ngOnDestroy(): void {
    if (this.searchSubscription) {
      this.searchSubscription.unsubscribe();
    }
  }

  onSearchChange(query: string) {
    this.searchService.updateSearchQuery(query);
  }
}
