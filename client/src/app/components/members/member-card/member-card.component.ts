import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {Member} from '../../../_models/member';
import {MembersService} from '../../../_services/members.service';
import {PresenceService} from '../../../_services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
})
export class MemberCardComponent implements OnInit {
  @Input() member: Member;
  @Output() dislikeUser = new EventEmitter<string>();

  constructor(
    private readonly membersService: MembersService,
    private readonly toastr: ToastrService,
    readonly presenceService: PresenceService
  ) {
  }

  ngOnInit() {
  }

  addLike(member: Member) {
    this.membersService.addLike(member.userName).subscribe(() => {
      this.member.likedByCurrentUser = true;
      this.toastr.success('You have liked ' + member.knownAs);
    });
  }

  removeLike(member: Member) {
    this.membersService.removeLike(member.userName).subscribe(() => {
      this.member.likedByCurrentUser = false;
      this.toastr.success('You have removed like for ' + member.knownAs);
      this.dislikeMember(member.userName);
    });
  }

  dislikeMember(username: string) {
    this.dislikeUser.emit(username);
  }
}
