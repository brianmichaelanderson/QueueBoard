import { ResolveFn } from '@angular/router';
import { QueueDto } from '../shared/models/queue';
import { inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { QueueService } from '../services/queue.service';

// Resolver returns list data from the API via QueueService; falls back to empty list on error.
export interface QueuesResolveResult { items: QueueDto[]; total: number; item?: QueueDto }

export const queuesResolver: ResolveFn<QueuesResolveResult> = async (_route, _state) => {
  const queueService = inject(QueueService);
  const id = _route.paramMap.get('id');
  try {
    if (id) {
      // Resolve a single item for detail routes
      const resp = await firstValueFrom(queueService.get(id));
      const item = resp?.item ?? null;
      return { items: [], total: 0, item: item ?? undefined };
    }
    const res = await firstValueFrom(queueService.list('', 1, 25));
    return { items: res.items ?? [], total: res.total ?? 0 };
  } catch {
    return { items: [], total: 0 };
  }
};
