import { TestBed, ComponentFixture } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { RouterTestingModule } from '@angular/router/testing';
import { QueuesListComponent } from './queues-list.component';
import { ActivatedRoute } from '@angular/router';
import { QueueDto } from '../shared/models/queue';
import { QueueService } from '../services/queue.service';
import { SEARCH_DEBOUNCE_MS } from './queues-list.component';
import { of } from 'rxjs';

describe('QueuesListComponent', () => {
  let fixture: ComponentFixture<QueuesListComponent>;
  let component: QueuesListComponent;

  beforeEach(async () => {
    const queueSvc = jasmine.createSpyObj('QueueService', ['list']);
    queueSvc.list.and.returnValue(of({ items: [{ id: '1', name: 'Support', description: 'desc' } as QueueDto], total: 1 }));

    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, QueuesListComponent],
      providers: [
        provideZonelessChangeDetection(),
        { provide: QueueService, useValue: queueSvc },
        { provide: SEARCH_DEBOUNCE_MS, useValue: 0 },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              data: { initialData: { items: [{ id: '1', name: 'Support', description: 'desc' } as QueueDto] } }
            }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(QueuesListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('loads items from resolver and renders list', () => {
    expect(component.items.length).toBe(1);
    expect(component.items[0].name).toBe('Support');

    const el = fixture.nativeElement as HTMLElement;
    const items = el.querySelectorAll('.queue-item');
    expect(items.length).toBe(1);
    expect(el.textContent).toContain('Support');
  });

  it('calls onSearch when typing in the search input', () => {
    spyOn(component, 'onSearch');
    const el = fixture.nativeElement as HTMLElement;
    const input = el.querySelector('input[aria-label="Search queues"]') as HTMLInputElement;
    expect(input).toBeTruthy();

    input.value = 'sup';
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    expect(component.onSearch).toHaveBeenCalled();
  });

  it('debounces input and calls QueueService.list with search param', async () => {
    const queueSvc = TestBed.inject(QueueService) as jasmine.SpyObj<QueueService>;

    // Emit into the debounced pipeline
    (component as any).search$.next('sup');
    fixture.detectChanges();

    // Wait one macrotask to let debounce/scheduler run
    await new Promise(resolve => setTimeout(resolve, 0));

    expect(queueSvc.list).toHaveBeenCalled();
    const args = queueSvc.list.calls.mostRecent().args;
    expect(args[0]).toBe('sup');
    expect(args[1]).toBe(1);
    expect(args[2]).toBe(25);
  });

  // Pagination tests (4.5.2) - TDD: tests added first and expected to fail until UI implemented
  it('calls QueueService.list with page param when changing page', async () => {
    const queueSvc = TestBed.inject(QueueService) as jasmine.SpyObj<QueueService>;
    const el = fixture.nativeElement as HTMLElement;

    const nextBtn = el.querySelector('.pagination .next') as HTMLButtonElement | null;
    expect(nextBtn).toBeTruthy(); // should exist once pagination UI is added

    nextBtn!.click();
    // allow any async pipeline to run
    await new Promise(r => setTimeout(r, 0));

    expect(queueSvc.list).toHaveBeenCalled();
    const args = queueSvc.list.calls.mostRecent().args;
    expect(args[1]).toBe(2); // page 2
  });

  it('calls QueueService.list with pageSize when changing page size', async () => {
    const queueSvc = TestBed.inject(QueueService) as jasmine.SpyObj<QueueService>;
    const el = fixture.nativeElement as HTMLElement;

    const pageSizeSelect = el.querySelector('.page-size-select') as HTMLSelectElement | null;
    expect(pageSizeSelect).toBeTruthy(); // should exist once page-size UI is added

    pageSizeSelect!.value = '50';
    pageSizeSelect!.dispatchEvent(new Event('change'));
    await new Promise(r => setTimeout(r, 0));

    expect(queueSvc.list).toHaveBeenCalled();
    const args = queueSvc.list.calls.mostRecent().args;
    expect(args[2]).toBe(50); // pageSize 50
  });
});
