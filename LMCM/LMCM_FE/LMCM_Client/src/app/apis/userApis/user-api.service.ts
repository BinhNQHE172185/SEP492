import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

interface LoginResponse {
  message: string;
  token: string;
  user:{id: string}
}

@Injectable({
  providedIn: 'root'
})
export class UserApiService {

  constructor(private http: HttpClient) { }

  private apiUrl = 'http://localhost:5035/api/User';

  login(token: string): Observable<LoginResponse> {
    const loginData = { token };
    return this.http.post<any>(`${this.apiUrl}/google-login`, loginData);
  }
}
