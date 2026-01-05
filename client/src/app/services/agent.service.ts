import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { API_BASE_URL } from '../app.tokens';

// Lightweight AgentService skeleton — replace placeholder implementations
// with real API calls once `Agent` DTOs and backend endpoints are available.
@Injectable({ providedIn: 'root' })
export class AgentService {
  private http = inject(HttpClient);
  private apiBase = inject(API_BASE_URL);
  private baseUrl = `${this.apiBase}/agents`;

  getAll(): Observable<any[]> {
    // TODO: return this.http.get<any[]>(this.baseUrl);
    return of([]);
  }

  getById(id: string): Observable<any | null> {
    return this.http.get<any>(`${this.baseUrl}/${id}`, { observe: 'response' }).pipe(
      map(resp => {
        const body = resp.body ?? null;
        const rawEtag = resp.headers.get('ETag') ?? resp.headers.get('etag') ?? undefined;
        const etag = rawEtag ? rawEtag.replace(/"/g, '') : undefined;
        if (body && etag) {
          body.rowVersion = etag;
        }
        return body;
      })
    );
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
