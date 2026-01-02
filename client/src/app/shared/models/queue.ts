export interface QueueDto {
  id: string;
  name: string;
  description?: string;
  // Optional rowVersion / ETag token (header wins if both provided)
  rowVersion?: string;
}
