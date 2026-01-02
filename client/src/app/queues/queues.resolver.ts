import { ResolveFn } from '@angular/router';

// Lightweight resolver stub that returns mock/placeholder data for the queues list.
// Replace with a real API call via `QueueService` when available.
export const queuesResolver: ResolveFn<any> = async (_route, _state) => {
  // Example: return an empty list placeholder until QueueService is implemented
  return { items: [], total: 0 };
};
