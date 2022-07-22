export interface Message {
  id: number;
  senderId: string;
  senderName: string;
  senderPhotoUrl: string;
  recipientId: string;
  recipientName: string;
  recipientPhotoUrl: string;
  content: string;
  dateRead: Date;
  dateSent: Date;
}
