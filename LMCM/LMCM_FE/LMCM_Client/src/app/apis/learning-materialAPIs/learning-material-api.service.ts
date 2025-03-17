import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

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
export class LearningMaterialApiService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getLearningMaterial(request: PagingRequest): Observable<PagedResult<any>> {
    return this.http.post<PagedResult<any>>(`${this.apiUrl}/LearningMaterial/getChangesHistoryList`, request);
  }
  createLearningMaterialHistory(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/LearningMaterial/createChangesHistory`, data);
  }
  getLearningMaterialList(id: any): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/LearningMaterial/getMaterialsList?syllabusId=${id}`);
  }
  getLearningMaterialById(id: any): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/LearningMaterial/${id}`);
  }
}
