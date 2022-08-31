import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {Login} from '../../_models/login';
import {AccountService} from '../../_services/account.service';
import {Roles} from '../../Enums/roles';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  model: Login;
  rolesEnum = Roles;
  isShown = false;

  constructor(public accountService: AccountService, private router: Router) {
    this.model = new class implements Login {
      password: string;
      username: string;
    }();
  }

  ngOnInit() {
  }

  login() {
    this.accountService.login(this.model).subscribe({
      next: () => this.router.navigateByUrl('/members'),
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
