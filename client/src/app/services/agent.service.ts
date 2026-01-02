import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';

// Lightweight AgentService skeleton — replace placeholder implementations
// with real API calls once `Agent` DTOs and backend endpoints are available.
@Injectable({ providedIn: 'root' })
export class AgentService {
  private baseUrl = '/api/agents';

  private http = inject(HttpClient);

  getAll(): Observable<any[]> {
    // TODO: return this.http.get<any[]>(this.baseUrl);
    return of([]);
  }

  getById(_id: string): Observable<any | null> {
    // TODO: return this.http.get<any>(`${this.baseUrl}/${_id}`);
    return of(null);
  }

  create(_payload: any): Observable<any> {
    // TODO: return this.http.post<any>(this.baseUrl, _payload);
    return of(null);
  }

  update(_id: string, _payload: any): Observable<any> {
    // TODO: return this.http.put<any>(`${this.baseUrl}/${_id}`, _payload);
    return of(null);
  }

  delete(_id: string): Observable<void> {
    // TODO: return this.http.delete<void>(`${this.baseUrl}/${_id}`);
    return of(void 0);
  }
}
