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
export class DocumentTemplateApiService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

   getTemplates(request: PagingRequest): Observable<PagedResult<any>> {
     return this.http.post<PagedResult<any>>(`${this.apiUrl}/documentTemplate/getTemplatesList`, request);
   }
 

  getTemplateDetail(id: any): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/documentTemplate/getTemplateDetail?id=${id}`);
  }

  createTemplate(request: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/documentTemplate/createTemplate`, request);
  }

  updateTemplate(id: any, request: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/documentTemplate/updateTemplate/${id}`, request);
  }

  deleteTemplate(templateId: any): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/documentTemplate/deleteTemplate/${templateId}`);
  }
}
