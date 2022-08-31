import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {Router} from '@angular/router';
import {Login} from '../../../_models/login';
import {AccountService} from '../../../_services/account.service';

@Component({
  selector: 'app-learn-more',
  templateUrl: './learn-more.component.html',
  styleUrls: ['./learn-more.component.css'],
})
export class LearnMoreComponent implements OnInit {
  @Output() cancelLearnMore: EventEmitter<boolean> = new EventEmitter();

  constructor(
    readonly accountService: AccountService,
    private readonly router: Router
  ) {
  }

  ngOnInit() {
  }

  cancel() {
    this.cancelLearnMore.emit(false);
  }

  demoLogin(username: string) {
    const loginModel: Login = new class implements Login {
      password = 'Pa$$word';
      username = username;
    }();

    this.accountService.login(loginModel).subscribe({
      next: () => this.router.navigateByUrl('/members'),
    });
  }
}
