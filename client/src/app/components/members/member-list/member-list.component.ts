import {MembersService} from '../../../_services/members.service';
import {Member} from '../../../_models/member';
import {Component, OnInit} from '@angular/core';
import {Pagination} from "../../../_models/pagination";
import {UserParams} from "../../../_models/userParams";
import {AccountService} from "../../../_services/account.service";
import {User} from "../../../_models/user";
import {take} from "rxjs";

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members: Member[];
  pagination: Pagination;
  userParams: UserParams;
  user: User;

  constructor(private memberService: MembersService, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user: User) => {
        this.user = user;
        this.userParams = new UserParams(user);
      }
    })
  }

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {
    this.memberService.getMembers(this.userParams).subscribe({
        next: response => {
          this.members = response.result;
          this.pagination = response.pagination;
        }
      }
    )
  }

  pageChanged(event: any) {
    this.userParams.pageNumber = event.page;
    this.loadMembers();
  }
}
