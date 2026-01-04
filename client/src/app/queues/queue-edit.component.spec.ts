import { TestBed, ComponentFixture } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { of } from 'rxjs';
import { QueueEditComponent } from './queue-edit.component';
import { QueueService } from '../services/queue.service';
import { ActivatedRoute, Router } from '@angular/router';
import { QueueDto } from '../shared/models/queue';

describe('QueueEditComponent', () => {
  let fixture: ComponentFixture<QueueEditComponent>;
  let component: QueueEditComponent;
  let queueService: jasmine.SpyObj<QueueService>;
  let router: { navigate: jasmine.Spy };

  beforeEach(() => {
    queueService = jasmine.createSpyObj('QueueService', ['get', 'update', 'create']);
    router = { navigate: jasmine.createSpy('navigate') };

    TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, QueueEditComponent],
      providers: [
        provideZonelessChangeDetection(),
        { provide: QueueService, useValue: queueService },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: { get: (k: string) => '1' },
              data: {
                initialData: {
                  item: { id: '1', name: 'Support', description: 'desc', rowVersion: 'r1' } as QueueDto
                }
              }
            }
          }
        },
        { provide: Router, useValue: router }
      ]
    });

    fixture = TestBed.createComponent(QueueEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should initialize form from resolver data', () => {
    expect(component.isEdit).toBeTrue();
    const formValue = component.form.value;
    expect(formValue.name).toBe('Support');
    expect(formValue.description).toBe('desc');
  });

  it('should call update on submit when editing and navigate on success', () => {
    component.isEdit = true;
    component.id = '1';
    component.form.patchValue({ name: 'Updated', description: 'updated desc' });
    // set private etag for the update
    (component as any).etag = 'r1';

    queueService.update.and.returnValue(of(void 0));

    component.onSubmit();

    expect(queueService.update).toHaveBeenCalled();
    const args = queueService.update.calls.mostRecent().args;
    expect(args[0]).toBe('1');
    expect(args[1]).toEqual({ name: 'Updated', description: 'updated desc' });
    expect(args[2]).toBe('r1');
    expect(router.navigate).toHaveBeenCalledWith(['/queues']);
  });
});
