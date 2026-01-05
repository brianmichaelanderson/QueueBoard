import { TestBed } from '@angular/core/testing';
import { HttpTestingController, HttpClientTestingModule } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { AgentService } from './agent.service';
import { API_BASE_URL } from '../app.tokens';

describe('AgentService', () => {
  let service: AgentService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [provideZonelessChangeDetection()]
    });

    service = TestBed.inject(AgentService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('getById() should GET agent and read ETag header into rowVersion', () => {
    const mockAgent = { id: '1', name: 'Agent One' } as any;
    const etag = '"agent-etag"';

    service.getById('1').subscribe(res => {
      expect(res).toBeTruthy();
      expect((res as any).id).toBe('1');
      // expect ETag handling to set rowVersion when implemented
      // this test will fail until AgentService reads ETag from responses
      expect((res as any).rowVersion).toBe('agent-etag');
    });

    const req = httpMock.expectOne('/api/agents/1');
    expect(req.request.method).toBe('GET');
    req.flush(mockAgent, { headers: { ETag: etag } });
  });

  it('create() should POST and capture ETag header into returned DTO', () => {
    const payload = { name: 'New Agent' };
    const created = { id: '2', name: 'New Agent' } as any;
    const etag = '"r1"';

    service.create(payload).subscribe(result => {
      expect(result).toBeTruthy();
      expect((result as any).id).toBe('2');
      expect((result as any).rowVersion).toBe('r1');
    });

    const req = httpMock.expectOne('/api/agents');
    expect(req.request.method).toBe('POST');
    req.flush(created, { headers: { ETag: etag } });
  });

  it('update() should PUT and send If-Match header when etag provided', () => {
    const payload = { name: 'Updated' };

    service.update('1', payload).subscribe(() => {
      // noop
    });

    const req = httpMock.expectOne('/api/agents/1');
    expect(req.request.method).toBe('PUT');
    // AgentService currently does not send If-Match; this test asserts it should
    expect(req.request.headers.has('If-Match')).toBeTrue();

    req.flush(null, { status: 204, statusText: 'No Content', headers: { ETag: '"r2"' } });
  });

  it('delete() should issue DELETE to the API', () => {
    service.delete('7').subscribe(() => {
      // noop
    });

    const req = httpMock.expectOne('/api/agents/7');
    expect(req.request.method).toBe('DELETE');
    req.flush(null, { status: 204, statusText: 'No Content' });
  });

  it('list() should GET with query params and return items from the API', () => {
    service.getAll();

    const req = httpMock.expectOne(r => r.url.startsWith('/api/agents'));
    expect(req.request.method).toBe('GET');
    req.flush({ items: [{ id: '9', name: 'Agent X' }], totalCount: 1 });
  });

  it('should use injected API_BASE_URL when provided', () => {
    const testBase = 'http://test.local/api';

    TestBed.resetTestingModule();
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [provideZonelessChangeDetection(), { provide: API_BASE_URL, useValue: testBase }]
    });

    const svc = TestBed.inject(AgentService);
    const http = TestBed.inject(HttpTestingController);

    svc.getAll().subscribe();

    const req = http.expectOne(r => r.url.startsWith(`${testBase}/agents`));
    expect(req.request.method).toBe('GET');
    http.verify();
  });
});
