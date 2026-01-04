import { Component, inject, OnDestroy, InjectionToken } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { QueueDto } from '../shared/models/queue';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { QueueService } from '../services/queue.service';

export const SEARCH_DEBOUNCE_MS = new InjectionToken<number>('SEARCH_DEBOUNCE_MS');

@Component({
  standalone: true,
  selector: 'app-queues-list',
  imports: [CommonModule, RouterModule],
  template: `
    <!-- eslint-disable @angular-eslint/template/prefer-control-flow -->
    <div class="app-container">
      <main class="app-main">
        <h1 class="page-title">Queues</h1>

        <div class="controls">
          <input aria-label="Search queues" placeholder="Search queues" (input)="onSearch($event)" />
          <a class="create-link" [routerLink]="['/queues', 'create']">Create queue</a>
        </div>

        <ul class="queue-list">
          <li *ngFor="let q of items" class="queue-item">
            <a [routerLink]="['/queues', 'edit', q.id]">{{ q.name }}</a>
            <p class="muted">{{ q.description }}</p>
          </li>
        </ul>

        <p *ngIf="items.length === 0" class="empty">No queues found.</p>

        <div class="pagination" style="margin-top:1rem; display:flex; gap:1rem; align-items:center">
          <button class="prev" (click)="prevPage()" [disabled]="page<=1">Previous</button>
          <button class="next" (click)="nextPage()">Next</button>

          <label style="margin-left:1rem">Page size
            <select class="page-size-select" (change)="onPageSizeChange($event)">
              <option value="10">10</option>
              <option value="25" selected>25</option>
              <option value="50">50</option>
            </select>
          </label>
        </div>
      </main>
    </div>
  `,
  styles: [
    `:host { display: block; }`,
    `.page-title { margin-bottom: 0.5rem; }`,
    `.controls { display:flex; gap:1rem; align-items:center; margin-bottom:1rem }`,
    `.queue-list { list-style:none; padding:0; margin:0 }`,
    `.queue-item { padding:0.5rem 0; border-bottom:1px solid #eee }`,
    `.muted { color: #666; margin:0; font-size:0.9rem }`,
    `.empty { color:#666 }`
  ]
})
export class QueuesListComponent {
  private route = inject(ActivatedRoute);
  private queueService = inject(QueueService);
  private search$ = new Subject<string>();
  private sub?: Subscription;

  items: QueueDto[] = [];
  page = 1;
  pageSize = 25;

  constructor() {
    const data = this.route.snapshot.data as { initialData?: { items?: QueueDto[] } };
    if (data?.initialData?.items) {
      this.items = data.initialData.items;
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
      .pipe(debounceTime(debounceMs), distinctUntilChanged(), switchMap(term => this.queueService.list(term, 1, this.pageSize)))
      .subscribe(res => {
        this.items = res.items;
      });
  }

  onSearch(event: Event) {
    const v = (event.target as HTMLInputElement).value ?? '';
    this.page = 1; // reset to first page on new search
    this.search$.next(v);
  }

  private loadPage(search = '') {
    // call list with current page and pageSize
    this.queueService.list(search, this.page, this.pageSize).subscribe(res => {
      this.items = res.items;
    });
  }

  nextPage() {
    this.page += 1;
    this.loadPage();
  }

  prevPage() {
    if (this.page > 1) {
      this.page -= 1;
      this.loadPage();
    }
  }

  onPageSizeChange(ev: Event) {
    const v = (ev.target as HTMLSelectElement).value;
    const n = parseInt(v, 10) || 25;
    this.pageSize = n;
    this.page = 1;
    this.loadPage();
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }
}
