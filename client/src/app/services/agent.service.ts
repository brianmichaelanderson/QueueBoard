import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';

// Lightweight AgentService skeleton — replace placeholder implementations
// with real API calls once `Agent` DTOs and backend endpoints are available.
@Injectable({ providedIn: 'root' })
export class AgentService {
  private baseUrl = '/api/agents';

  constructor(private http: HttpClient) {}

  getAll(): Observable<any[]> {
    // TODO: return this.http.get<any[]>(this.baseUrl);
    return of([]);
  }

  getById(id: string): Observable<any | null> {
    // TODO: return this.http.get<any>(`${this.baseUrl}/${id}`);
    return of(null);
  }

  create(payload: any): Observable<any> {
    // TODO: return this.http.post<any>(this.baseUrl, payload);
    return of(null);
  }

  update(id: string, payload: any): Observable<any> {
    // TODO: return this.http.put<any>(`${this.baseUrl}/${id}`, payload);
    return of(null);
  }

  delete(id: string): Observable<void> {
    // TODO: return this.http.delete<void>(`${this.baseUrl}/${id}`);
    return of(void 0);
  }
}
