import { take } from 'rxjs';
import { MembersService } from './../../../_services/members.service';
import { AccountService } from './../../../_services/account.service';
import { Component, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {

  member: Member;
  user: User;

  constructor(private accountService: AccountService, private memberService: MembersService) {
    accountService.currentUser$.pipe(take(1))
      .subscribe(user => this.user = user);
   }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    this.memberService.getMember(this.user.name).subscribe({
      next: member => this.member = member
    });
  }

}
