import { Component, inject, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { AgentDto } from '../shared/models/agent.model';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, finalize } from 'rxjs/operators';
import { AgentService } from '../services/agent.service';
import { SEARCH_DEBOUNCE_MS } from '../queues/queues-list.component';

@Component({
  standalone: true,
  selector: 'app-agent-list',
  imports: [CommonModule, RouterModule],
  template: `
    <div class="app-container">
      <a class="skip-link" href="#main">Skip to content</a>
      <main id="main" class="app-main">
        <h1 class="page-title">Agents</h1>

        <div class="controls">
          <input aria-label="Search agents" placeholder="Search agents" (input)="onSearch($event)" [disabled]="loading" aria-controls="agent-list" />
          <a class="create-link" [routerLink]="['/agents', 'create']">Create agent</a>
        </div>

        <div aria-live="polite" role="status" class="sr-only" *ngIf="loading">Loading agents…</div>

        <ul id="agent-list" class="agent-list">
          <ng-container *ngIf="loading; else itemsBlock">
            <li *ngFor="let _ of skeletons" class="agent-item skeleton">
              <div class="skeleton-title"></div>
              <div class="skeleton-desc"></div>
            </li>
          </ng-container>
          <ng-template #itemsBlock>
            <li *ngFor="let a of items" class="agent-item">
              <a [routerLink]="['/agents','view', a.id]">{{ a.firstName }} {{ a.lastName }}</a>
              <p class="muted">{{ a.email }}</p>
            </li>
          </ng-template>
        </ul>

        <p *ngIf="!loading && items.length === 0" class="empty">No agents found.</p>

        <div class="pagination" role="navigation" aria-label="Pagination" style="margin-top:1rem; display:flex; gap:1rem; align-items:center">
          <button class="prev" (click)="prevPage()" [disabled]="loading || page<=1" aria-label="Previous page">Previous</button>
          <button class="next" (click)="nextPage()" [disabled]="loading || (total>0 && page>=totalPages)" aria-label="Next page">Next</button>
        </div>
        <div style="margin-top:.5rem">Page {{page}} of {{totalPages}}</div>
      </main>
    </div>
  `,
  styleUrls: ['./agent-list.component.scss']
})
export class AgentListComponent implements OnDestroy {
  private route = inject(ActivatedRoute);
  private agentService = inject(AgentService);
  private cdr = inject(ChangeDetectorRef);
  private search$ = new Subject<string>();
  private sub?: Subscription;

  items: AgentDto[] = [];
  page = 1;
  pageSize = 25;
  total = 0;
  totalPages = 1;
  loading = false;
  skeletons = Array.from({ length: 5 });
  currentSearch = '';

  constructor() {
    const data = this.route.snapshot.data as { initialData?: { items?: AgentDto[]; total?: number } };
    if (data?.initialData?.items) {
      this.items = data.initialData.items;
      this.total = (data as any).initialData.total ?? this.total;
      this.totalPages = Math.max(1, Math.ceil(this.total / this.pageSize));
    } else {
      this.items = [
        { id: '1', firstName: 'Agent', lastName: 'A', email: 'a@x.com', isActive: true, createdAt: new Date().toISOString() },
        { id: '2', firstName: 'Agent', lastName: 'B', email: 'b@x.com', isActive: true, createdAt: new Date().toISOString() }
      ];
    }

    const debounceMs = ((): number => {
      try {
        return (inject(SEARCH_DEBOUNCE_MS, { optional: true }) as number) ?? 300;
      } catch {
        return 300;
      }
    })();

    this.sub = this.search$.pipe(debounceTime(debounceMs), distinctUntilChanged()).subscribe(term => {
      this.currentSearch = term;
      this.page = 1;
      this.loadPage(this.currentSearch);
    });
  }

  onSearch(event: Event) {
    const v = (event.target as HTMLInputElement).value ?? '';
    this.page = 1;
    this.search$.next(v);
  }

  private loadPage(search = '') {
    const term = search ?? this.currentSearch ?? '';
    this.loading = true;
    this.agentService
      .list(term, this.page, this.pageSize)
      .pipe(finalize(() => {
        this.loading = false;
        try { this.cdr.detectChanges(); } catch { /* noop */ }
      }))
      .subscribe(res => {
        this.items = res.items ?? [];
        this.total = res.total ?? 0;
        this.totalPages = Math.max(1, Math.ceil(this.total / this.pageSize));
      }, () => {
        this.items = [];
        this.total = 0;
        this.totalPages = 1;
      });
  }

  nextPage() {
    this.page += 1;
    this.loadPage(this.currentSearch);
  }

  prevPage() {
    if (this.page > 1) {
      this.page -= 1;
      this.loadPage(this.currentSearch);
    }
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }
}

