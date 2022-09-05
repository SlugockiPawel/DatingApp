import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions,} from '@kolkov/ngx-gallery';
import {TabDirective, TabsetComponent} from 'ngx-bootstrap/tabs';
import {ToastrService} from 'ngx-toastr';
import {take} from 'rxjs';
import {Member} from '../../../_models/member';
import {Message} from '../../../_models/message';
import {User} from '../../../_models/user';
import {AccountService} from '../../../_services/account.service';
import {MembersService} from '../../../_services/members.service';
import {MessageService} from '../../../_services/message.service';
import {PresenceService} from '../../../_services/presence.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent;
  activeTab: TabDirective;
  member: Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  messages: Message[] = [];
  user: User;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly messageService: MessageService,
    readonly presenceService: PresenceService,
    private readonly accountService: AccountService,
    private readonly router: Router,
    private readonly membersService: MembersService,
    private readonly toastr: ToastrService
  ) {
    this.accountService.currentUser$
      .pipe(take(1))
      .subscribe(user => this.user = user);

    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.member = data.member;
    });

    this.route.queryParams.subscribe(params =>
      params.tab ? this.selectTab(params.tab) : this.selectTab(0)
    );

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false,
      },
    ];

    this.galleryImages = this.getImages();
  }

  getImages(): NgxGalleryImage[] {
    const imageUrls = [];
    this.member.photos.forEach(photo =>
      imageUrls.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url,
      })
    );

    return imageUrls;
  }

  loadMessages() {
    this.messageService
      .getMessageThread(this.member.userName)
      .subscribe(messages => {
        this.messages = messages;
      });
  }

  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.messages.length === 0) {
      this.messageService.createHubConnection(this.user, this.member.userName);
    } else {
      this.messageService.stopHubConnection();
    }
  }

  ngOnDestroy() {
    this.messageService.stopHubConnection();
  }

  addLike(member: Member) {
    this.membersService
      .addLike(member.userName)
      .subscribe(() => this.toastr.success('You have liked ' + member.knownAs));
  }
}
