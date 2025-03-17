import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class searchService {

  private searchQuerySubject = new BehaviorSubject<string>('');
  searchQuery$ = this.searchQuerySubject.asObservable();

  updateSearchQuery(query: string): void {
    if (query !== this.searchQuerySubject.getValue()) {
      this.searchQuerySubject.next(query);
    }
  }
}
