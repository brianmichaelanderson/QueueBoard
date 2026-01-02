export interface QueueDto {
  id: string; // GUID
  name: string;
  description?: string | null;
  isActive: boolean;
  createdAt: string; // ISO datetime
  rowVersion?: string | null; // base64 token
}

export interface CreateQueueDto {
  name: string;
  description?: string | null;
  isActive: boolean;
}

export interface UpdateQueueDto {
  name: string;
  description?: string | null;
  isActive: boolean;
  rowVersion?: string | null;
}
