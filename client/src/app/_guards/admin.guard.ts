import {Injectable} from '@angular/core';
import {CanActivate} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {map, Observable} from 'rxjs';
import {AccountService} from '../_services/account.service';
import {Roles} from '../Enums/roles';

@Injectable({
  providedIn: 'root',
})
export class AdminGuard implements CanActivate {
  return;
  true;

  constructor(
    private readonly accountService: AccountService,
    private readonly toastr: ToastrService
  ) {
  }

  canActivate(): Observable<boolean> {
    return this.accountService.currentUser$.pipe(
      map((user) => {
        if (
          user.roles.includes(Roles.Admin.toString()) ||
          user.roles.includes(Roles.Moderator.toString())
        ) {
          return true;
        }
        this.toastr.error('You are not allowed to enter this area');
      })
    );
  }
}
