import { TestBed, ComponentFixture } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { RouterTestingModule } from '@angular/router/testing';
import { AgentListComponent } from './agent-list.component';
import { ActivatedRoute } from '@angular/router';
import { AgentDto } from '../shared/models/agent.model';
import { AgentService } from '../services/agent.service';
import { SEARCH_DEBOUNCE_MS } from '../queues/queues-list.component';
import { of } from 'rxjs';

describe('AgentListComponent (5.4.1 + 5.5.1-3 tests)', () => {
  let fixture: ComponentFixture<AgentListComponent>;
  let component: AgentListComponent;

  beforeEach(async () => {
    const agentSvc = jasmine.createSpyObj('AgentService', ['list']);
    agentSvc.list.and.returnValue(of({ items: [{ id: '1', firstName: 'Agent', lastName: 'One', email: 'a@x.com', isActive: true, createdAt: new Date().toISOString() } as AgentDto], total: 1 }));

    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, AgentListComponent],
      providers: [
        provideZonelessChangeDetection(),
        { provide: AgentService, useValue: agentSvc },
        { provide: SEARCH_DEBOUNCE_MS, useValue: 0 },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              data: { initialData: { items: [{ id: '1', firstName: 'Agent', lastName: 'One', email: 'a@x.com', isActive: true, createdAt: new Date().toISOString() } as AgentDto] }, showEditButtons: true }
            }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AgentListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('loads items from resolver and renders list', () => {
    expect((component as any).items.length).toBeGreaterThanOrEqual(1);

    const el = fixture.nativeElement as HTMLElement;
    const items = el.querySelectorAll('.agent-item');
    expect(items.length).toBeGreaterThan(0);
    expect(el.textContent).toContain('Agent');
  });

  it('calls onSearch when typing in the search input (TDD - expected to fail until implemented)', () => {
    spyOn(component as any, 'onSearch');
    const el = fixture.nativeElement as HTMLElement;
    const input = el.querySelector('input[aria-label="Search agents"]') as HTMLInputElement;
    expect(input).toBeTruthy();

    input.value = 'agent';
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    expect((component as any).onSearch).toHaveBeenCalled();
  });

  it('debounces input and calls AgentService.list with search param (TDD - expected to fail)', async () => {
    const agentSvc = TestBed.inject(AgentService) as jasmine.SpyObj<AgentService>;

    // Emit into the debounced pipeline if present
    (component as any).search$?.next?.('agent');
    fixture.detectChanges();

    await new Promise(resolve => setTimeout(resolve, 0));

    expect(agentSvc.list).toHaveBeenCalled();
    const args = agentSvc.list.calls.mostRecent().args;
    expect(args[0]).toBe('agent');
    expect(args[1]).toBe(1);
    expect(args[2]).toBe(25);
  });

  it('calls AgentService.list with page param when changing page (TDD - expected to fail)', async () => {
    const agentSvc = TestBed.inject(AgentService) as jasmine.SpyObj<AgentService>;
    const el = fixture.nativeElement as HTMLElement;

    const nextBtn = el.querySelector('.pagination .next') as HTMLButtonElement | null;
    expect(nextBtn).toBeTruthy();

    nextBtn!.click();
    await new Promise(r => setTimeout(r, 0));

    expect(agentSvc.list).toHaveBeenCalled();
    const args = agentSvc.list.calls.mostRecent().args;
    expect(args[1]).toBe(2);
  });

  it('honors admin context and sets baseRoute to /admin when showEditButtons is true', () => {
    expect((component as any).baseRoute).toBe('/admin');

    const el = fixture.nativeElement as HTMLElement;
    const createLink = el.querySelector('.create-link') as HTMLAnchorElement | null;
    expect(createLink).toBeTruthy();
    // href may be rendered as absolute path; assert it contains the admin create path
    expect(createLink!.getAttribute('href') || '').toContain('/admin');
  });

  it('hides create link when not in admin context', async () => {
    await TestBed.resetTestingModule();
    const agentSvc = jasmine.createSpyObj('AgentService', ['list']);
    agentSvc.list.and.returnValue(of({ items: [{ id: '1', firstName: 'Agent', lastName: 'One', email: 'a@x.com', isActive: true, createdAt: new Date().toISOString() } as AgentDto], total: 1 }));

    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, AgentListComponent],
      providers: [
        provideZonelessChangeDetection(),
        { provide: AgentService, useValue: agentSvc },
        { provide: SEARCH_DEBOUNCE_MS, useValue: 0 },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              data: { initialData: { items: [{ id: '1', firstName: 'Agent', lastName: 'One', email: 'a@x.com', isActive: true, createdAt: new Date().toISOString() } as AgentDto] }, showEditButtons: false }
            }
          }
        }
      ]
    }).compileComponents();

    const fx = TestBed.createComponent(AgentListComponent);
    fx.detectChanges();
    const el = fx.nativeElement as HTMLElement;
    const createLink = el.querySelector('.create-link') as HTMLAnchorElement | null;
    expect(createLink).toBeNull();
  });
});
