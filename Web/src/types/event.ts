export type Event = {
  id?: string;
  name: string;
  description: string;
  startDate: number;
  endDate: number;
  startDateStr?: string;
  endDateStr?: string;
  isPublic: boolean;
  visible: boolean;
  eventCode?: string;
  createdBy?: string;
  isPublicText?: string;
  visibleText?: string;
  updated_at?: string;
};
