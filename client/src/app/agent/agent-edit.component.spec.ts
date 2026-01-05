import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { provideZonelessChangeDetection } from '@angular/core';
import { AgentEditComponent } from './agent-edit.component';
import { AgentService } from '../services/agent.service';
import { ActivatedRoute, Router } from '@angular/router';

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
});
