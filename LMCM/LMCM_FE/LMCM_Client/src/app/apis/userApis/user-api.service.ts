import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
// import { environment } from '../../../environments/environment.prod';

interface LoginResponse {
  message: string;
  data: {
    id: string;
    token: string;
  }
}

export interface PagingRequest {
  searchKey?: string;
  pageNumber: number;
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
export class UserApiService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  login(token: string): Observable<LoginResponse> {
    const loginData = { token };
    return this.http.post<LoginResponse>(`${this.apiUrl}/User/google-login`, loginData, {
      withCredentials: true // Ensures cookies are sent and received
    });
  }

  createAccount(staffId: string): Observable<any> {
    const data = { staffId };
    return this.http.post<any>(`${this.apiUrl}/User/create-account`, data);
  }

  getListUser(request: PagingRequest): Observable<PagedResult<any>> {
    return this.http.post<any>(`${this.apiUrl}/User/list-user`, request);
  }
  getProfile(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/User/profile`, { withCredentials: true });
  }
  changeRole(userId: string, newRole: string): Observable<any> {
    const data = { userId, newRole };
    return this.http.post<any>(`${this.apiUrl}/User/assign-role`, data);
  }
}
