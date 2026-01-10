import { TestBed, ComponentFixture } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
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
    expect(router.navigate).toHaveBeenCalledWith(['/queues', 'view', '1']);
  });

  it('applies server validation errors to form controls and shows alert on 412', () => {
    component.isEdit = true;
    component.id = '1';
    (component as any).etag = 'r1';

    spyOn(window, 'alert');

    const validationBody = { errors: { name: ['Name required'] } } as any;
    // ensure form is valid so submit proceeds
    component.form.patchValue({ name: 'Some name', description: 'desc' });
    queueService.update.and.returnValue(throwError(() => new HttpErrorResponse({ status: 400, error: validationBody })));

    component.onSubmit();
    fixture.detectChanges();

    const nameErrors = component.form.controls.name.errors as any;
    expect(nameErrors).toBeTruthy();
    expect(nameErrors.server).toBe('Name required');

    // clear server errors so the form becomes valid again and submit proceeds
    component.form.controls.name.setErrors(null);
    component.form.setErrors(null);

    // now simulate 412 Precondition Failed
    queueService.update.and.returnValue(throwError(() => new HttpErrorResponse({ status: 412 })));
    component.onSubmit();
    expect(window.alert).toHaveBeenCalledWith('This queue has been modified by another user. Please reload and try again.');
  });

  it('navigates to admin routes on save and cancel when showEditButtons is true', async () => {
    // reset and configure module with admin route data and spy router
    await TestBed.resetTestingModule();

    const queueService = jasmine.createSpyObj('QueueService', ['get', 'update', 'create']);
    const router = { navigate: jasmine.createSpy('navigate') };

    queueService.update.and.returnValue(of(void 0));

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, QueueEditComponent],
      providers: [
        provideZonelessChangeDetection(),
        { provide: QueueService, useValue: queueService },
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: (k: string) => '1' }, data: { initialData: { item: { id: '1', name: 'Support', description: 'desc', rowVersion: 'r1' } as QueueDto }, showEditButtons: true } } } },
        { provide: Router, useValue: router }
      ]
    }).compileComponents();

    const fx = TestBed.createComponent(QueueEditComponent);
    fx.detectChanges();
    const cmp = fx.componentInstance;

    cmp.isEdit = true;
    cmp.id = '1';
    (cmp as any).etag = 'r1';
    cmp.form.patchValue({ name: 'Updated', description: 'u' });

    cmp.onSubmit();
    expect(queueService.update).toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/admin','queues','1']);

    // test cancel navigation
    cmp.onCancel();
    expect(router.navigate).toHaveBeenCalledWith(['/admin','queues','1']);
  });
});
