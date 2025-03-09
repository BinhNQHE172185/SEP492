import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HttpHeaders } from '@angular/common/http';
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

  deleteSyllabus(syllabusId: string): Observable<any> {
    return this.http.post(
      `${this.apiUrl}/Syllabus/deleteSyllabus`,
      syllabusId,  // Không dùng JSON.stringify
      {
        headers: new HttpHeaders({ 'Content-Type': 'application/json' })
      }
    );
  }
  
  
}
