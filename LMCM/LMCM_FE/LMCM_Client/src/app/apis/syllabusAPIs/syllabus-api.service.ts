import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface PagingRequest {
  searchKey?: string;
  pageIndex: number;
  pageSize: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  totalPages: number;
  pageSize: number;
  currentPage: number;
}

@Injectable({
  providedIn: 'root'
})
export class SyllabusApiService {
  private apiUrl = environment.apiUrl;
  
  constructor(private http: HttpClient) { }

  getSyllabuses(request: PagingRequest): Observable<PagedResult<any>> {
    return this.http.post<PagedResult<any>>(`${this.apiUrl}/Syllabus/getSyllabusesList`, request);
  }
  importSyllabuses(request: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/Syllabus/importSyllabus`, request);
  }
}
