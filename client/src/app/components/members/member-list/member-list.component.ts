import {MembersService} from '../../../_services/members.service';
import {Member} from '../../../_models/member';
import {Component, OnInit} from '@angular/core';
import {Pagination} from "../../../_models/pagination";

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members: Member[];
  pagination: Pagination;
  pageNumber = 1;
  pageSize = 5;

  constructor(private memberService: MembersService) {
  }

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {
    this.memberService.getMembers(this.pageNumber, this.pageSize).subscribe({
        next: response => {
          this.members = response.result;
          this.pagination = response.pagination;
        }
      }
    )
  }
}
