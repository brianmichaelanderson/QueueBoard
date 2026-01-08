import { TestBed } from '@angular/core/testing';
import { Router, ActivatedRoute, RouterLinkWithHref } from '@angular/router';
import { By } from '@angular/platform-browser';
import { AgentListComponent } from './agent-list.component';
import { Component, Directive, Input } from '@angular/core';
import { Subject, of } from 'rxjs';
import { AgentService } from '../services/agent.service';

@Component({ standalone: true, template: '' })
class DummyDetailComponent {}

@Directive({ selector: '[routerLink]' })
class RouterLinkStub {
  @Input('routerLink') routerLink: any;
}

describe('AgentListComponent navigation', () => {
  // Use Angular's default NgZone for this unit-style spec.

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [AgentListComponent, DummyDetailComponent],
      providers: [
        { provide: AgentService, useValue: { list: () => of({ items: [], total: 0 }) } },
        { provide: Router, useValue: { navigate: jasmine.createSpy('navigate'), navigateByUrl: jasmine.createSpy('navigateByUrl') } },
        { provide: ActivatedRoute, useValue: { snapshot: { data: {} } } }
      ]
    });
  });

  xit('builds a routerLink to view/:id for the first item', () => {
    const fixture = TestBed.createComponent(AgentListComponent as any);
    fixture.detectChanges();

    const linkDe = fixture.debugElement.query(By.css('#agent-list a'));
    expect(linkDe).toBeTruthy();

    const rl = linkDe.injector.get(RouterLinkStub) as any;
    expect(rl).toBeTruthy();
    expect(rl.routerLink).toEqual(['/agents', 'view', '1']);
  });
});
