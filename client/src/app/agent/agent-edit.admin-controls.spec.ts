import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { provideZonelessChangeDetection } from '@angular/core';
import { AgentEditComponent } from './agent-edit.component';
import { AgentService } from '../services/agent.service';
import { AuthService } from '../services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';

describe('AgentEditComponent admin controls (5.2.2)', () => {
  function createFixtureFor(routeId: string | null) {
    const mockAgentService: jasmine.SpyObj<AgentService> = jasmine.createSpyObj('AgentService', ['getById', 'create', 'update']);
    mockAgentService.getById.and.returnValue(of({ id: '1', firstName: 'A', lastName: 'B', email: 'a@x.com', isActive: true } as any));

    const mockAuthService: Partial<AuthService> = { isAdmin: () => of(false) } as any;

    TestBed.configureTestingModule({
      imports: [AgentEditComponent],
      providers: [
        provideZonelessChangeDetection(),
        { provide: AgentService, useValue: mockAgentService },
        { provide: AuthService, useValue: mockAuthService },
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: () => routeId } } } },
        { provide: Router, useValue: { navigate: jasmine.createSpy('navigate') } }
      ]
    });

    const fixture = TestBed.createComponent(AgentEditComponent);
    const component = fixture.componentInstance;
    return { fixture, component, mockAgentService };
  }

  it('disables Save for non-admin on create when form is valid', () => {
    const { fixture, component } = createFixtureFor(null);

    // Make the form valid
    component.form.setValue({ firstName: 'New', lastName: 'Agent', email: 'n@example.com', isActive: true });

    fixture.detectChanges();

    const saveBtn: HTMLButtonElement | null = fixture.nativeElement.querySelector('button[type="submit"]');
    expect(saveBtn).toBeTruthy();
    // Expect disabled because non-admin users should not be able to save (test will fail until feature implemented)
    expect(saveBtn!.disabled).toBeTrue();
  });

  it('disables Save for non-admin on edit when form is valid', () => {
    const { fixture, component } = createFixtureFor('1');

    // Trigger ngOnInit to load agent
    fixture.detectChanges();

    component.form.setValue({ firstName: 'Loaded', lastName: 'Agent', email: 'l@example.com', isActive: true });
    fixture.detectChanges();

    const saveBtn: HTMLButtonElement | null = fixture.nativeElement.querySelector('button[type="submit"]');
    expect(saveBtn).toBeTruthy();
    // Expect disabled because non-admin users should not be able to save edits (test will fail until feature implemented)
    expect(saveBtn!.disabled).toBeTrue();
  });
});
