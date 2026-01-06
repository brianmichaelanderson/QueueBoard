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
    mockAgentService.getById.and.returnValue(of({ id: '1', name: 'Agent One', rowVersion: 'r1' }));

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
    // Expect the component to call AgentService.getById and patch the form with the returned name
    expect(mockAgentService.getById).toHaveBeenCalledWith('1');
    const name = component.form.get('name')!.value;
    expect(name).toBe('Agent One');
  });

  it('should call update on save when editing', () => {
    fixture.detectChanges();
    component.form.setValue({ name: 'Updated Name' });
    component.save();
    expect(mockAgentService.update).toHaveBeenCalledWith('1', jasmine.objectContaining({ name: 'Updated Name' }), 'r1');
  });

  it('applies server validation errors to form controls and shows alert on 412', () => {
    fixture.detectChanges();
    component.form.patchValue({ name: 'Some name' });

    spyOn(window, 'alert');

    const validationBody = { errors: { name: ['Name required'] } } as any;
    mockAgentService.update.and.returnValue(throwError(() => new HttpErrorResponse({ status: 400, error: validationBody })));

    component.save();
    // After error handling, the form control should have server errors
    const nameErrors = component.form.controls.name.errors as any;
    expect(nameErrors).toBeTruthy();
    expect(nameErrors.server).toBe('Name required');

    // Now simulate 412 Precondition Failed
    mockAgentService.update.and.returnValue(throwError(() => new HttpErrorResponse({ status: 412 })));
    component.save();
    expect((window as any).alert).toHaveBeenCalled();
  });
});
