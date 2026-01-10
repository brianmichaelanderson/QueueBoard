import { Component, inject, OnDestroy, InjectionToken, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { QueueDto } from '../shared/models/queue';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, finalize } from 'rxjs/operators';
import { QueueService } from '../services/queue.service';

export const SEARCH_DEBOUNCE_MS = new InjectionToken<number>('SEARCH_DEBOUNCE_MS');

@Component({
  standalone: true,
  selector: 'app-queues-list',
  imports: [CommonModule, RouterModule],
  template: `
    <!-- eslint-disable @angular-eslint/template/prefer-control-flow -->
    <div class="app-container">
      <a class="skip-link" href="#main">Skip to content</a>
      <main id="main" class="app-main">
        <h1 class="page-title">Queues</h1>

        <div class="controls">
          <input aria-label="Search queues" placeholder="Search queues" (input)="onSearch($event)" [disabled]="loading" />
          <a class="home-link" [routerLink]="['/']">Home</a>
          <a class="create-link" [routerLink]="isAdmin ? ['/admin','queues','create'] : ['/queues','create']">Create queue</a>
        </div>

        <div aria-live="polite" class="sr-only" *ngIf="loading">Loading queues…</div>

        <ul class="queue-list">
          <ng-container *ngIf="loading; else itemsBlock">
            <li *ngFor="let _ of skeletons" class="queue-item skeleton">
              <div class="skeleton-title"></div>
              <div class="skeleton-desc"></div>
            </li>
          </ng-container>
          <ng-template #itemsBlock>
            <li *ngFor="let q of items" class="queue-item">
              <a [routerLink]="isAdmin ? ['/admin','queues', q.id] : ['/queues','view', q.id]">{{ q.name }}</a>
              <p class="muted">{{ q.description }}</p>
            </li>
          </ng-template>
        </ul>

        <p *ngIf="!loading && items.length === 0" class="empty">No queues found.</p>

        <div class="pagination" style="margin-top:1rem; display:flex; gap:1rem; align-items:center">
          <button class="prev" (click)="prevPage()" [disabled]="loading || page<=1" aria-label="Previous page">Previous</button>
          <button class="next" (click)="nextPage()" [disabled]="loading || (total>0 && page>=totalPages)" aria-label="Next page">Next</button>

          <!-- page-size selector removed for MVP simplicity -->
        </div>
        <div style="margin-top:.5rem">Page {{page}} of {{totalPages}}</div>
      </main>
    </div>
  `,
  styleUrls: ['./queues-list.component.scss']
})
export class QueuesListComponent {
  private route = inject(ActivatedRoute);
  isAdmin = false;
  private queueService = inject(QueueService);
  private cdr = inject(ChangeDetectorRef);
  private search$ = new Subject<string>();
  private sub?: Subscription;
  currentSearch = '';

  items: QueueDto[] = [];
  page = 1;
  pageSize = 25;
  total = 0;
  totalPages = 1;
  loading = false;
  skeletons = Array.from({ length: 5 });

  constructor() {
    const data = this.route.snapshot.data as { initialData?: { items?: QueueDto[] } };
    if (data?.initialData?.items) {
      this.items = data.initialData.items;
      // initialize pagination totals from resolver-provided data when available
      this.total = (data as any).initialData.total ?? this.total;
      this.totalPages = Math.max(1, Math.ceil(this.total / this.pageSize));
      this.isAdmin = !!((data as any).showEditButtons);
    } else {
      // Placeholder sample data for local dev
      this.items = [
        { id: '1', name: 'Support', description: 'Customer support queue' },
        { id: '2', name: 'Sales', description: 'Sales inquiries' }
      ];
    }
    console.log('QueuesListComponent resolver initialData:', data?.initialData);

    // wire debounced search -> service.list
    const debounceMs = ((): number => {
      try {
        // optional injection: tests can override SEARCH_DEBOUNCE_MS
        return (inject(SEARCH_DEBOUNCE_MS, { optional: true }) as number) ?? 300;
      } catch {
        return 300;
      }
    })();

    this.sub = this.search$
      .pipe(debounceTime(debounceMs), distinctUntilChanged())
      .subscribe(term => {
        this.currentSearch = term;
        this.page = 1; // reset to first page for new searches
        this.loadPage(this.currentSearch);
      });
  }

  onSearch(event: Event) {
    const v = (event.target as HTMLInputElement).value ?? '';
    this.page = 1; // reset to first page on new search
    this.search$.next(v);
  }

  private loadPage(search = '') {
    const term = search ?? this.currentSearch ?? '';
    this.loading = true;
    // call list with current page and pageSize
    this.queueService
      .list(term, this.page, this.pageSize)
      .pipe(finalize(() => {
        this.loading = false;
        try { this.cdr.detectChanges(); } catch { /* noop */ }
      }))
      .subscribe(res => {
        this.items = res.items;
        this.total = res.total ?? 0;
        this.totalPages = Math.max(1, Math.ceil(this.total / this.pageSize));
        console.log(`QueuesListComponent.loadPage: page=${this.page} total=${this.total} items=${this.items.length} first=${this.items[0]?.name ?? 'n/a'}`);
      }, err => {
        // keep empty list on error
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

  onPageSizeChange(ev: Event) {
    // removed: page-size selector was intentionally removed
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }
}
