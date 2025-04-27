import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ContractValueApiComponent {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getContractValue(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/contract-value-items/list`);
  }
  updateContractValue(request: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/contract-value-items/update`, request, {withCredentials: true});
  }
}
