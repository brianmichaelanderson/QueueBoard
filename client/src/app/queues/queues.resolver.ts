import { ResolveFn } from '@angular/router';
import { QueueDto } from '../shared/models/queue';

// Lightweight resolver stub that returns mock/placeholder data for the queues list.
// Replace with a real API call via `QueueService` when available.
export interface QueuesResolveResult { items: QueueDto[]; total: number; item?: QueueDto }

export const queuesResolver: ResolveFn<QueuesResolveResult> = async (_route, _state) => {
  // Example: return an empty list placeholder until QueueService is implemented
  return { items: [], total: 0 };
};
