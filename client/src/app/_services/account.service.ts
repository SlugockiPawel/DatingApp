import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {map, ReplaySubject} from 'rxjs';
import {environment} from 'src/environments/environment';
import {User} from '../_models/user';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private baseurl: string = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) {
  }

  registerUser(model: any) {
    return this.http.post(this.baseurl + 'account/register', model).pipe(
      map((user: User) => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  login(model: any) {
    return this.http.post(this.baseurl + 'account/login', model).pipe(
      map((response: User) => {
        const user: User = response;
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}
