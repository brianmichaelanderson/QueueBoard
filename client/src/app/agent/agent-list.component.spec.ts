import { TestBed } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { AgentListComponent } from './agent-list.component';

describe('AgentListComponent (5.4.1)', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [AgentListComponent],
      providers: [provideZonelessChangeDetection()]
    });
  });

  it('renders a search input for filtering agents', () => {
    const fixture = TestBed.createComponent(AgentListComponent);
    fixture.detectChanges();
    const el: HTMLElement = fixture.nativeElement;
    const input = el.querySelector('input[type="search"], input.search-input');
    expect(input).toBeTruthy();
  });

  it('renders agent items and pagination controls with edit/detail links', () => {
    const fixture = TestBed.createComponent(AgentListComponent);
    fixture.detectChanges();
    const el: HTMLElement = fixture.nativeElement;

    const list = el.querySelector('.agent-list');
    expect(list).toBeTruthy();

    const items = el.querySelectorAll('.agent-item');
    expect(items.length).toBeGreaterThan(0);

    const editLink = el.querySelector('a.agent-edit-link');
    const detailLink = el.querySelector('a.agent-detail-link');
    expect(editLink).toBeTruthy();
    expect(detailLink).toBeTruthy();

    const pagination = el.querySelector('.pagination');
    expect(pagination).toBeTruthy();
  });
});
