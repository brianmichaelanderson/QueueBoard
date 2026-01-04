import { TestBed } from '@angular/core/testing';
import { HttpTestingController, HttpClientTestingModule } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { QueueService } from './queue.service';
import { QueueDto } from '../shared/models/queue';
import { API_BASE_URL } from '../app.tokens';

describe('QueueService', () => {
  let service: QueueService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [provideZonelessChangeDetection()]
    });

    service = TestBed.inject(QueueService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('get() should GET queue and read ETag header into rowVersion', () => {
    const mockQueue: QueueDto = { id: '1', name: 'Test', description: 'desc' };
    const etag = '"etag-token"';

    service.get('1').subscribe(res => {
      expect(res).toBeTruthy();
      expect(res?.item).toBeTruthy();
      expect(res?.item?.id).toBe('1');
      expect(res?.item?.rowVersion).toBe('etag-token');
      expect(res?.etag).toBe('etag-token');
    });

    const req = httpMock.expectOne('/api/queues/1');
    expect(req.request.method).toBe('GET');
    req.flush(mockQueue, { headers: { ETag: etag } });
  });

  it('create() should POST and capture ETag header into returned DTO', () => {
    const payload: Partial<QueueDto> = { name: 'New' };
    const created: QueueDto = { id: '2', name: 'New' };
    const etag = '"r1"';

    service.create(payload).subscribe(result => {
      expect(result).toBeTruthy();
      expect(result.id).toBe('2');
      expect(result.rowVersion).toBe('r1');
    });

    const req = httpMock.expectOne('/api/queues');
    expect(req.request.method).toBe('POST');
    req.flush(created, { headers: { ETag: etag } });
  });

  it('update() should PUT and send If-Match header when etag provided', () => {
    const payload: Partial<QueueDto> = { name: 'Updated' };

    service.update('1', payload, 'r1').subscribe(() => {
      // noop
    });

    const req = httpMock.expectOne('/api/queues/1');
    expect(req.request.method).toBe('PUT');
    expect(req.request.headers.has('If-Match')).toBeTrue();
    expect(req.request.headers.get('If-Match')).toBe('r1');

    req.flush(null, { status: 204, statusText: 'No Content', headers: { ETag: '"r2"' } });
  });

  it('get() should handle responses with no ETag', () => {
    const mockQueue: QueueDto = { id: '3', name: 'NoEtag', description: 'none' };

    service.get('3').subscribe(res => {
      expect(res).toBeTruthy();
      expect(res?.item).toBeTruthy();
      expect(res?.item?.id).toBe('3');
      expect(res?.item?.rowVersion).toBeUndefined();
      expect(res?.etag).toBeUndefined();
    });

    const req = httpMock.expectOne('/api/queues/3');
    expect(req.request.method).toBe('GET');
    req.flush(mockQueue);
  });

  it('update() should NOT send If-Match header when etag not provided', () => {
    const payload: Partial<QueueDto> = { name: 'NoIfMatch' };

    service.update('5', payload).subscribe(() => {
      // noop
    });

    const req = httpMock.expectOne('/api/queues/5');
    expect(req.request.method).toBe('PUT');
    expect(req.request.headers.has('If-Match')).toBeFalse();

    req.flush(null, { status: 204, statusText: 'No Content' });
  });

  it('delete() should issue DELETE to the API', () => {
    service.delete('7').subscribe(() => {
      // noop
    });

    const req = httpMock.expectOne('/api/queues/7');
    expect(req.request.method).toBe('DELETE');
    req.flush(null, { status: 204, statusText: 'No Content' });
  });

  it('list() should GET with query params and return items from the API', () => {
    service.list('support', 2, 10).subscribe(res => {
      expect(res).toBeTruthy();
      expect(res.items.length).toBe(1);
      expect(res.total).toBe(1);
    });

    // Expect an HTTP request with query params - service currently returns a placeholder, so
    // this test should fail until `list` calls the API with query params.
    const req = httpMock.expectOne(r => r.url.startsWith('/api/queues'));
    expect(req.request.method).toBe('GET');
    expect(req.request.params.get('search')).toBe('support');
    expect(req.request.params.get('page')).toBe('2');
    expect(req.request.params.get('pageSize')).toBe('10');

    req.flush({ items: [{ id: '9', name: 'Support' } as QueueDto], total: 1 });
  });

  it('should use injected API_BASE_URL when provided', () => {
    const testBase = 'http://test.local/api';

    // Reconfigure TestBed to provide a test API base URL and recreate the service
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [provideZonelessChangeDetection(), { provide: API_BASE_URL, useValue: testBase }]
    });

    const svc = TestBed.inject(QueueService);
    const http = TestBed.inject(HttpTestingController);

    svc.list('abc', 1, 5).subscribe();

    // Expect the request to use the injected base URL.
    const req = http.expectOne(r => r.url.startsWith(`${testBase}/queues`));
    expect(req.request.method).toBe('GET');
    http.verify();
  });
});
