import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { QueueDto } from '../shared/models/queue';
import { API_BASE_URL } from '../app.tokens';

@Injectable({ providedIn: 'root' })
export class QueueService {
  private http = inject(HttpClient);
  private apiBase = inject(API_BASE_URL);
  private baseUrl = `${this.apiBase}/queues`;

  // Result wrapper when we need to include the ETag header value
  interfaceGetResult(item: QueueDto | null, etag?: string) {
    return { item, etag } as { item: QueueDto | null; etag?: string };
  }

  list(search?: string, page?: number, pageSize?: number): Observable<{ items: QueueDto[]; total: number }> {
    let params: { [key: string]: string } = {};
    if (search) params['search'] = search;
    if (page !== undefined && page !== null) params['page'] = String(page);
    if (pageSize !== undefined && pageSize !== null) params['pageSize'] = String(pageSize);

    return this.http.get<{ items: QueueDto[]; total?: number; totalCount?: number }>(this.baseUrl, { params }).pipe(
      map(resp => {
        const total = resp.total ?? resp.totalCount ?? 0;
        return { items: resp.items ?? [], total };
      })
    );
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
