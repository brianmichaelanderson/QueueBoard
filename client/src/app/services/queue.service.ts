import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { QueueDto } from '../shared/models/queue';

@Injectable({ providedIn: 'root' })
export class QueueService {
  private http = inject(HttpClient);
  private baseUrl = '/api/queues';

  // Result wrapper when we need to include the ETag header value
  interfaceGetResult(item: QueueDto | null, etag?: string) {
    return { item, etag } as { item: QueueDto | null; etag?: string };
  }

  list(_search?: string, _page?: number, _pageSize?: number): Observable<{ items: QueueDto[]; total: number }> {
    // TODO: add query params; placeholder until API is wired
    return of({ items: [], total: 0 });
  }

  /**
   * GET /api/queues/{id} — returns item and reads ETag header when present
   */
  get(id: string): Observable<{ item: QueueDto | null; etag?: string } | null> {
    return this.http
      .get<QueueDto>(`${this.baseUrl}/${id}`, { observe: 'response' })
      .pipe(
        map(resp => {
          const body = resp.body ?? null;
          const rawEtag = resp.headers.get('ETag') ?? resp.headers.get('etag') ?? undefined;
          const etag = rawEtag ? rawEtag.replace(/"/g, '') : undefined;
          if (body && etag) {
            body.rowVersion = etag;
          }
          return { item: body, etag };
        })
      );
  }

  /**
   * POST /api/queues — create. Returns created DTO and fills `rowVersion` from ETag header when present.
   */
  create(payload: Partial<QueueDto>): Observable<QueueDto> {
    return this.http.post<QueueDto>(this.baseUrl, payload, { observe: 'response' }).pipe(
      map(resp => {
        const body = resp.body as QueueDto;
        const rawEtag = resp.headers.get('ETag') ?? resp.headers.get('etag') ?? undefined;
        if (body && rawEtag) {
          body.rowVersion = rawEtag.replace(/"/g, '');
        }
        return body;
      })
    );
  }

  /**
   * PUT /api/queues/{id} — send `If-Match` header when `etag` provided. API may return 204 NoContent
   * with a new ETag header on success.
   */
  update(id: string, payload: Partial<QueueDto>, etag?: string): Observable<void> {
    const headers = etag ? new HttpHeaders({ 'If-Match': etag }) : undefined;
    return this.http.put(`${this.baseUrl}/${id}`, payload, { headers, observe: 'response' }).pipe(
      map(() => void 0)
    );
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
