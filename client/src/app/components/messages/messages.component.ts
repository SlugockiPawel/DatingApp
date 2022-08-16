import {Component, OnInit} from '@angular/core';
import {Message} from '../../_models/message';
import {Pagination} from '../../_models/pagination';
import {ConfirmService} from '../../_services/confirm.service';
import {MessageService} from '../../_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css'],
})
export class MessagesComponent implements OnInit {
  messages: Message[] = [];
  pagination: Pagination;
  container = 'Unread';
  pageNumber = 1;
  pageSize = 5;
  loading = false;

  constructor(
    private readonly messageService: MessageService,
    private readonly confirmService: ConfirmService
  ) {
  }

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages() {
    this.loading = true;
    this.messageService
      .getMessages(this.pageNumber, this.pageSize, this.container)
      .subscribe(response => {
        this.messages = response.result;
        this.pagination = response.pagination;
        this.loading = false;
      });
  }

  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadMessages();
    }
  }

  deleteMessage(id: number) {
    this.confirmService
      .confirm(
        'Confirm delete message',
        'Are you sure you want to delete the message?',
        'Yes'
      )
      .subscribe(result => {
        if (result) {
          return this.messageService.deleteMessage(id).subscribe(() =>
            this.messages.splice(
              this.messages.findIndex(m => m.id === id),
              1
            )
          );
        }
      });
  }
}
