import { TestBed, ComponentFixture } from '@angular/core/testing';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { Router } from '@angular/router';
import { provideZonelessChangeDetection } from '@angular/core';
import { of } from 'rxjs';
import { QueuesListComponent } from './queues-list.component';
import { QueueDto } from '../shared/models/queue';

@Component({ standalone: true, imports: [RouterOutlet], template: '<router-outlet></router-outlet>' })
class TestHostComponent {}

describe('Queues integration', () => {
  let fixture: ComponentFixture<TestHostComponent>;
  let router: Router;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule.withRoutes([
          {
            path: 'queues',
            component: QueuesListComponent,
            data: { initialData: { items: [{ id: '1', name: 'Support', description: 'desc' } as QueueDto] } }
          }
        ]),
        QueuesListComponent,
        TestHostComponent
      ],
      providers: [
        provideZonelessChangeDetection(),
        // provide a lightweight QueueService stub so the standalone component can inject it
        {
          provide: (await import('../services/queue.service')).QueueService,
          useValue: { list: () => of({ items: [{ id: '1', name: 'Support', description: 'desc' } as QueueDto], total: 1 }) }
        }
      ]
    }).compileComponents();

    router = TestBed.inject(Router);
    fixture = TestBed.createComponent(TestHostComponent);
    fixture.detectChanges();
  });

  it('navigates to /queues and shows resolver items', async () => {
    await router.navigateByUrl('/queues');
    fixture.detectChanges();
    await fixture.whenStable();

    const el = fixture.nativeElement as HTMLElement;
    expect(el.textContent).toContain('Support');
    const items = el.querySelectorAll('.queue-item');
    expect(items.length).toBe(1);
  });
});
