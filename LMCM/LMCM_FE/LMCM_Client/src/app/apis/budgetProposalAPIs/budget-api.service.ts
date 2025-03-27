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
export class BudgetApiService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getBudget(request: PagingRequest): Observable<PagedResult<any>> {
    return this.http.post<PagedResult<any>>(
      `${this.apiUrl}/budgetProposal/getBudgetProposalList`,
      request,
      { withCredentials: true } // Enable sending cookies
    );
  }

  getBudgetList(): Observable<PagedResult<any>> {
    return this.http.post<PagedResult<any>>(
      `${this.apiUrl}/budgetProposal/getBudgetProposalNoPagingList`,
      { withCredentials: true }
    );
  }

  createBudget(request: any): Observable<any> {
    return this.http.post<any>(
      `${this.apiUrl}/budgetProposal/createBudgetProposal`,
      request,
      { withCredentials: true }
    );
  }

  getBudgetDetail(proposalId: string): Observable<any> {
    return this.http.get<any>(
      `${this.apiUrl}/budgetProposal/getBudgetProposalDetail?proposalId=${proposalId}`,
      { withCredentials: true }
    );
  }

  updateBudget(id: string, request: any): Observable<any> {
    return this.http.put<any>(
      `${this.apiUrl}/budgetProposal/updateBudgetProposal/${id}`,
      request,
      { withCredentials: true }
    );
  }

  deleteBudget(id: string, authorId: string): Observable<any> {
    return this.http.delete<any>(
      `${this.apiUrl}/budgetProposal/deleteBudgetProposal/${id}`,
      { withCredentials: true }
    );
  }

}
