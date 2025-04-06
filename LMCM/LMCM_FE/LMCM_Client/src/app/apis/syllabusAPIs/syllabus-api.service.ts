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
  getSyllabusDetail(id: any): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/Syllabus/getSyllabusDetail?syllabusId=${id}`);
  }
  importSyllabuses(request: any, keepData: boolean): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/Syllabus/importSyllabus?keepUserCreated=${keepData}`, request);
  }
  deleteSyllabuses(id: any): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/Syllabus/${id}`);
  }
  getSyllabusList(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/Syllabus/getSyllabusesListNoPaging`);
  }
}
