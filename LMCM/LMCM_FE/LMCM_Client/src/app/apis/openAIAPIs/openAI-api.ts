import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OpenAIApiService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  analyzeUploadFile(file: File, prompt: string): Observable<any> {
    const formData = new FormData();
    formData.append("file", file);
    formData.append("prompt", prompt);
  
    return this.http.post<any>(`${this.apiUrl}/OpenAI/analyze-upload-file`, formData, {
      withCredentials: true
    });
  }
}
