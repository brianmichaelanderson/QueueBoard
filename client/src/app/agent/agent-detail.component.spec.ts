import { TestBed, ComponentFixture } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { RouterTestingModule } from '@angular/router/testing';
import { AgentDetailComponent } from './agent-detail.component';
import { ActivatedRoute } from '@angular/router';
import { AgentDto } from '../shared/models/agent.model';

describe('AgentDetailComponent (failing spec)', () => {
  let fixture: ComponentFixture<AgentDetailComponent>;
  let component: AgentDetailComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, AgentDetailComponent],
      providers: [
        provideZonelessChangeDetection(),
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: { get: (k: string) => '7' },
              data: { initialData: { item: { id: '7', firstName: 'Agent', lastName: 'Seven', email: 's@x.com', isActive: true, createdAt: new Date().toISOString() } as AgentDto } }
            }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AgentDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('shows resolved item details when present (expected to fail)', () => {
    // The component currently only renders the id; expecting resolved item details should fail
    expect(component.id).toBe('7');

    const el = fixture.nativeElement as HTMLElement;
    // Expecting name/details which the component does not render yet
    expect(el.textContent).toContain('Agent Seven');
    expect(el.textContent).toContain('s@x.com');
  });
});
