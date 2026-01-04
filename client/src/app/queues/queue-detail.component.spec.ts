import { TestBed, ComponentFixture } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { RouterTestingModule } from '@angular/router/testing';
import { QueueDetailComponent } from './queue-detail.component';
import { ActivatedRoute } from '@angular/router';
import { QueueDto } from '../shared/models/queue';

describe('QueueDetailComponent', () => {
  let fixture: ComponentFixture<QueueDetailComponent>;
  let component: QueueDetailComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, QueueDetailComponent],
      providers: [
        provideZonelessChangeDetection(),
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: { get: (k: string) => '42' },
              data: { initialData: { item: { id: '42', name: 'Support', description: 'desc' } as QueueDto } }
            }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(QueueDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('shows resolved item details when present', () => {
    expect(component.id).toBe('42');
    expect(component.item).toBeTruthy();
    expect(component.item?.name).toBe('Support');

    const el = fixture.nativeElement as HTMLElement;
    expect(el.textContent).toContain('Support');
    expect(el.textContent).toContain('desc');
    expect(el.textContent).toContain('42');
  });
});
