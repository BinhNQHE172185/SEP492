import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
// import { environment } from '../../../environments/environment.prod';

interface LoginResponse {
  message: string;
  token: string;
  user:{id: string}
}

@Injectable({
  providedIn: 'root'
})
export class UserApiService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  login(token: string): Observable<LoginResponse> {
    const loginData = { token };
    return this.http.post<any>(`${this.apiUrl}/User/google-login`, loginData);
  }
}
