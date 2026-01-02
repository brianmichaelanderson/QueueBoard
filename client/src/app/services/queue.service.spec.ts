import { TestBed } from '@angular/core/testing';
import { HttpTestingController, HttpClientTestingModule } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { QueueService } from './queue.service';
import { QueueDto } from '../shared/models/queue';

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
});
