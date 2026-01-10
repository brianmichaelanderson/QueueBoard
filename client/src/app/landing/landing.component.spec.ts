import { routes } from '../app.routes';
import { TestBed, ComponentFixture } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { LandingComponent } from './landing.component';
import { AuthService } from '../services/auth.service';

describe('Landing route', () => {
  it('registers root landing route', () => {
    const root = routes.find(r => r.path === '');
    expect(root).toBeDefined();
    expect(root && (root.loadComponent || root.component)).toBeDefined();
  });
});

describe('LandingComponent enterAdmin()', () => {
  let fixture: ComponentFixture<LandingComponent>;
  let component: LandingComponent;
  let authSpy: jasmine.SpyObj<AuthService>;
  let router: Router;

  beforeEach(async () => {
    authSpy = jasmine.createSpyObj('AuthService', ['becomeAdmin']);

    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, LandingComponent],
      providers: [provideZonelessChangeDetection(), { provide: AuthService, useValue: authSpy }]
    }).compileComponents();

    fixture = TestBed.createComponent(LandingComponent);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);
  });

  it('calls AuthService.becomeAdmin() and navigates to the admin path', () => {
    const navSpy = spyOn(router, 'navigate');

    component.enterAdmin(new MouseEvent('click'), '/admin');

    expect(authSpy.becomeAdmin).toHaveBeenCalled();
    expect(navSpy).toHaveBeenCalledWith(['/admin']);
  });
});
