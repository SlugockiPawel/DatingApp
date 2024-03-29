import {Component, OnInit} from '@angular/core';
import {Member} from '../../_models/member';
import {Pagination} from '../../_models/pagination';
import {MembersService} from '../../_services/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css'],
})
export class ListsComponent implements OnInit {
  members: Partial<Member[]>;
  predicate = 'liked';
  pageNumber = 1;
  pageSize = 5;
  pagination: Pagination;

  constructor(private readonly membersService: MembersService) {
  }

  ngOnInit() {
    this.loadLikes();
  }

  loadLikes() {
    this.membersService
      .getLikes(this.predicate, this.pageNumber, this.pageSize)
      .subscribe(response => {
        this.members = response.result;
        this.pagination = response.pagination;
      });
  }

  handlePageChanged(event: any) {
    this.pageNumber = event.page;
    this.loadLikes();
  }

  handleUserDislike($event: string) {
    this.members.splice(
      this.members.findIndex(m => m.userName === $event),
      1
    );

    if (this.membersService.memberCache.size > 0) {
      this.membersService.memberCache.clear();
    }
  }
}
