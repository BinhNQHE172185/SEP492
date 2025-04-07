import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DashboardApiService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getPiechartData(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/Dashboard/pie-chart`, { withCredentials: true });
  }
  getbarChartData(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/Dashboard/column-chart`, { withCredentials: true });
  }
}
