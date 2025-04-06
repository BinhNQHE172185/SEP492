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
export class HistoryOfChangeApiService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getLearningMaterialHistory(request: PagingRequest): Observable<PagedResult<any>> {
    return this.http.post<PagedResult<any>>(`${this.apiUrl}/LearningMaterialChangesHistory/getChangesHistoryList`, request,
      { withCredentials: true });
  }

  getHistoryById(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/LearningMaterialChangesHistory/getHistoryOfChangeDetail/${id}`,
      { withCredentials: true });
  }

  createLearningMaterialHistory(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/LearningMaterialChangesHistory/create`, data,
      { withCredentials: true });
  }

  updateLearningMaterialHistory(id: string, data: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/LearningMaterialChangesHistory/update/${id}`, data,
      { withCredentials: true });
  }

  deleteLearningMaterialHistory(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/LearningMaterialChangesHistory/${id}`,
      { withCredentials: true });
  }
}
