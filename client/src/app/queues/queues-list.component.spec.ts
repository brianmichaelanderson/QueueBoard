import { TestBed, ComponentFixture } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { RouterTestingModule } from '@angular/router/testing';
import { QueuesListComponent } from './queues-list.component';
import { ActivatedRoute } from '@angular/router';
import { QueueDto } from '../shared/models/queue';

describe('QueuesListComponent', () => {
  let fixture: ComponentFixture<QueuesListComponent>;
  let component: QueuesListComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, QueuesListComponent],
      providers: [
        provideZonelessChangeDetection(),
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              data: {
                initialData: { items: [{ id: '1', name: 'Support', description: 'desc' } as QueueDto] }
              }
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
});
