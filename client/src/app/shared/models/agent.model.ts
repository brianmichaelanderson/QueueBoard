export interface AgentDto {
  id: string; // GUID
  firstName: string;
  lastName: string;
  email: string;
  isActive: boolean;
  createdAt: string; // ISO datetime
  rowVersion?: string | null;
}

export interface CreateAgentDto {
  firstName: string;
  lastName: string;
  email: string;
  isActive: boolean;
}

export interface UpdateAgentDto {
  firstName: string;
  lastName: string;
  email: string;
  isActive: boolean;
  rowVersion?: string | null;
}
