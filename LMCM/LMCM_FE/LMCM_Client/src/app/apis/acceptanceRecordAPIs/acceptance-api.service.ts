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
export class AcceptanceRecordApiService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  // Lấy danh sách hồ sơ nghiệm thu (có phân trang)
  getAcceptanceRecords(request: PagingRequest): Observable<PagedResult<any>> {
    return this.http.post<PagedResult<any>>(
      `${this.apiUrl}/acceptance-records/getAcceptanceRecordList`,
      request,
      { withCredentials: true }
    );
  }

  // Tạo mới một hồ sơ nghiệm thu
  createAcceptanceRecord(request: any): Observable<any> {
    return this.http.post<any>(
      `${this.apiUrl}/acceptance-records/create`,
      request,
      { withCredentials: true }
    );
  }

  // Lấy thông tin chi tiết một hồ sơ nghiệm thu
  getAcceptanceRecordDetail(acceptanceId: string): Observable<any> {
    return this.http.get<any>(
      `${this.apiUrl}/acceptance-records/${acceptanceId}`,
      { withCredentials: true }
    );
  }

  // Cập nhật hồ sơ nghiệm thu
  updateAcceptanceRecord(id: string, request: any): Observable<any> {
    return this.http.put<any>(
      `${this.apiUrl}/acceptance-records/update/${id}`,
      request,
      { withCredentials: true }
    );
  }

  // Xóa hồ sơ nghiệm thu
  deleteAcceptanceRecord(id: string): Observable<any> {
    return this.http.delete<any>(
      `${this.apiUrl}/acceptance-records/${id}`,
      { withCredentials: true }
    );
  }
}
