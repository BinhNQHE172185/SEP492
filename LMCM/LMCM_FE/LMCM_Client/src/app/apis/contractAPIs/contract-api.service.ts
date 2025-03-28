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
export class ContractApiService {
    private apiUrl = environment.apiUrl;

    constructor(private http: HttpClient) { }

    getContract(request: PagingRequest): Observable<PagedResult<any>> {
        return this.http.post<PagedResult<any>>(
            `${this.apiUrl}/contracts/getContractList`,
            request,
            { withCredentials: true } // Enable sending cookies
        );
    }

    createContract(request: any): Observable<any> {
        return this.http.post<any>(
            `${this.apiUrl}/contracts/createContract`,
            request,
            { withCredentials: true }
        );
    }

    getContractDetail(id: string): Observable<any> {
        return this.http.get<any>(
            `${this.apiUrl}/contracts/getContractDetail?contractId=${id}`,
            { withCredentials: true }
        );
    }

    updateContract(id: string, request: any): Observable<any> {
        return this.http.put<any>(
            `${this.apiUrl}/contracts/updateContract/${id}`,
            request,
            { withCredentials: true }
        );
    }

    deleteContract(id: string): Observable<any> {
        return this.http.delete<any>(
            `${this.apiUrl}/contracts/deleteContract/${id}`,
            { withCredentials: true }
        );
    }
}
