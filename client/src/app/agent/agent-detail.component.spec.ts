import { TestBed, ComponentFixture } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { RouterTestingModule } from '@angular/router/testing';
import { Router } from '@angular/router';
import { AgentDetailComponent } from './agent-detail.component';
import { ActivatedRoute } from '@angular/router';
import { AgentDto } from '../shared/models/agent.model';

describe('AgentDetailComponent (failing spec)', () => {
  let fixture: ComponentFixture<AgentDetailComponent>;
  let component: AgentDetailComponent;
  let routerSpy: { navigate: jasmine.Spy };

  beforeEach(async () => {
    routerSpy = { navigate: jasmine.createSpy('navigate') };

    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, AgentDetailComponent],
      providers: [
        provideZonelessChangeDetection(),
        { provide: Router, useValue: routerSpy },
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

  it('shows resolved item details and buttons when present', () => {
    expect(component.id).toBe('7');

    const el = fixture.nativeElement as HTMLElement;
    expect(el.textContent).toContain('First name:');
    expect(el.textContent).toContain('Agent');
    expect(el.textContent).toContain('Last name:');
    expect(el.textContent).toContain('Seven');
    expect(el.textContent).toContain('Email:');
    expect(el.textContent).toContain('s@x.com');
    expect(el.textContent).toContain('Active:');
    // ID should be visible
    expect(el.textContent).toContain('ID:');

    // Buttons present
    const buttons = Array.from(el.querySelectorAll('button')) as HTMLButtonElement[];
    const editBtn = buttons.find(b => b.textContent?.trim() === 'Edit');
    const cancelBtn = buttons.find(b => b.textContent?.trim() === 'Cancel');
    expect(editBtn).toBeTruthy();
    expect(cancelBtn).toBeTruthy();

    // Simulate clicks and expect navigation calls
    editBtn!.click();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/agents', 'edit', '7']);

    cancelBtn!.click();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/agents']);
  });
});
