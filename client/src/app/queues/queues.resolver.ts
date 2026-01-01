import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';

// Lightweight resolver stub that returns mock/placeholder data for the queues list.
// Replace with a real API call via `QueueService` when available.
export const queuesResolver: ResolveFn<any> = async (route, state) => {
  // Example: return an empty list placeholder until QueueService is implemented
  return { items: [], total: 0 };
};
