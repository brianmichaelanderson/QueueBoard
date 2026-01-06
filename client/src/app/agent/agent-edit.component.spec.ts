import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { provideZonelessChangeDetection } from '@angular/core';
import { AgentEditComponent } from './agent-edit.component';
import { AgentService } from '../services/agent.service';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

describe('AgentEditComponent (5.3.1)', () => {
  let fixture: any;
  let component: any;
  let mockAgentService: jasmine.SpyObj<AgentService>;

  beforeEach(() => {
    mockAgentService = jasmine.createSpyObj('AgentService', ['getById', 'update']);
    mockAgentService.getById.and.returnValue(of({ id: '1', firstName: 'Agent', lastName: 'One', email: 'a@x.com', isActive: true, rowVersion: 'r1' }));

    TestBed.configureTestingModule({
      imports: [AgentEditComponent],
      providers: [
        provideZonelessChangeDetection(),
        { provide: AgentService, useValue: mockAgentService },
        // Provide real tokens for ActivatedRoute + Router with minimal stubs
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: () => '1' } } } },
        { provide: Router, useValue: { navigate: jasmine.createSpy('navigate') } }
      ]
    });

    fixture = TestBed.createComponent(AgentEditComponent);
    component = fixture.componentInstance;
  });

  it('should load agent into form when editing', () => {
    fixture.detectChanges();
    // Expect the component to call AgentService.getById and patch the form with the returned values
    expect(mockAgentService.getById).toHaveBeenCalledWith('1');
    const first = component.form.get('firstName')!.value;
    const last = component.form.get('lastName')!.value;
    expect(first).toBe('Agent');
    expect(last).toBe('One');
  });

  it('should call update on save when editing', () => {
    fixture.detectChanges();
    component.form.setValue({ firstName: 'Updated', lastName: 'Name', email: 'u@example.com', isActive: true });
    component.save();
    expect(mockAgentService.update).toHaveBeenCalledWith('1', jasmine.objectContaining({ firstName: 'Updated', lastName: 'Name' }), 'r1');
  });

  it('should call create on save when creating a new agent', () => {
    // Simulate create mode (no id) without re-running ngOnInit
    component.isEdit = false;
    component.id = null;

    component.form.setValue({ firstName: 'New', lastName: 'Agent', email: 'n@example.com', isActive: true });

    mockAgentService.create = jasmine.createSpy('create').and.returnValue(of({ id: '10', firstName: 'New', lastName: 'Agent' } as any));

    component.save();

    expect((mockAgentService.create as jasmine.Spy).calls.any()).toBeTrue();
    const args = (mockAgentService.create as jasmine.Spy).calls.mostRecent().args[0];
    expect(args).toEqual(jasmine.objectContaining({ firstName: 'New', lastName: 'Agent' }));
  });

  it('applies server validation errors to form controls and shows alert on 412', () => {
    fixture.detectChanges();
    component.form.patchValue({ firstName: 'Some', lastName: 'Name', email: 's@example.com' });

    spyOn(window, 'alert');

    const validationBody = { errors: { firstName: ['First name required'] } } as any;
    mockAgentService.update.and.returnValue(throwError(() => new HttpErrorResponse({ status: 400, error: validationBody })));

    component.save();
    // After error handling, the form control should have server errors
    const firstErrors = component.form.controls.firstName.errors as any;
    expect(firstErrors).toBeTruthy();
    expect(firstErrors.server).toBe('First name required');

    // Now simulate 412 Precondition Failed
    mockAgentService.update.and.returnValue(throwError(() => new HttpErrorResponse({ status: 412 })));
    component.save();
    expect((window as any).alert).toHaveBeenCalled();
  });
});
