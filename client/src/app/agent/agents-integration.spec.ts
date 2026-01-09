import { TestBed, ComponentFixture } from '@angular/core/testing';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import createRouterTestModule from '../../test-helpers/router-test-helpers';
import { Router } from '@angular/router';
import { provideZonelessChangeDetection } from '@angular/core';
import { of } from 'rxjs';
import { AgentListComponent } from './agent-list.component';
import { AgentDto } from '../shared/models/agent.model';

@Component({ standalone: true, imports: [RouterOutlet], template: '<router-outlet></router-outlet>' })
class TestHostComponent {}

describe('Agents integration', () => {
  let fixture: ComponentFixture<TestHostComponent>;
  let router: Router;

  beforeEach(async () => {
    const cfg = createRouterTestModule([
      {
        path: 'agents',
        component: AgentListComponent,
        data: { initialData: { items: [{ id: '1', firstName: 'Agent', lastName: 'One', email: 'a@x.com', isActive: true, createdAt: new Date().toISOString() } as AgentDto] } }
      }
    ], { useFakeNgZone: true });

    await TestBed.configureTestingModule({
      imports: [
        ...(cfg.imports || []),
        AgentListComponent,
        TestHostComponent
      ],
      providers: [
        ...(cfg.providers || []),
        provideZonelessChangeDetection(),
        // provide a lightweight AgentService stub so the standalone component can inject it
        {
          provide: (await import('../services/agent.service')).AgentService,
          useValue: { list: () => of({ items: [{ id: '1', firstName: 'Agent', lastName: 'One', email: 'a@x.com', isActive: true, createdAt: new Date().toISOString() } as AgentDto], total: 1 }) }
        }
      ]
    }).compileComponents();

    router = TestBed.inject(Router);
    fixture = TestBed.createComponent(TestHostComponent);
    fixture.detectChanges();
  });

  it('navigates to /agents and shows resolver items', async () => {
    await router.navigateByUrl('/agents');
    fixture.detectChanges();
    await fixture.whenStable();

    const el = fixture.nativeElement as HTMLElement;
    expect(el.textContent).toContain('Agent');
    const items = el.querySelectorAll('.agent-item');
    expect(items.length).toBeGreaterThan(0);
  });
});
