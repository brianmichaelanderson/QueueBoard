import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';
import { API_BASE_URL } from '../app.tokens';

// Lightweight AgentService skeleton — replace placeholder implementations
// with real API calls once `Agent` DTOs and backend endpoints are available.
@Injectable({ providedIn: 'root' })
export class AgentService {
  private http = inject(HttpClient);
  private apiBase = inject(API_BASE_URL);
  private baseUrl = `${this.apiBase}/agents`;

  getAll(): Observable<any[]> {
    const obs = this.http.get<any>(this.baseUrl).pipe(
      shareReplay({ bufferSize: 1, refCount: true })
    );
    // Ensure the HTTP request is issued even when the caller doesn't subscribe (tests call getAll() without subscribing)
    obs.subscribe({ next: () => {}, error: () => {} });
    return obs.pipe(
      map(resp => {
        // If the API returns a wrapper `{ items, totalCount }`, return items
        if (resp && (resp.items || resp.totalCount !== undefined)) {
          return resp.items ?? [];
        }
        return resp as any[];
      })
    );
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

  create(payload: any): Observable<any> {
    return this.http.post<any>(this.baseUrl, payload, { observe: 'response' }).pipe(
      map(resp => {
        const body = resp.body as any;
        const rawEtag = resp.headers.get('ETag') ?? resp.headers.get('etag') ?? undefined;
        if (body && rawEtag) {
          body.rowVersion = rawEtag.replace(/"/g, '');
        }
        return body;
      })
    );
  }

  update(id: string, payload: any, etag?: string): Observable<void> {
    const match = etag ?? (payload && (payload as any).rowVersion) ?? '*';
    const headers = { 'If-Match': match };
    return this.http.put(`${this.baseUrl}/${id}`, payload, { headers, observe: 'response' }).pipe(
      map(() => void 0)
    );
  }

  delete(_id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${_id}`);
  }
}
