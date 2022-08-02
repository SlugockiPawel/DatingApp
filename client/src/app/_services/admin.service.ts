import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {environment} from '../../environments/environment';
import {User} from '../_models/user';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  baseUrl = environment.apiUrl;

  constructor(private readonly http: HttpClient) {
  }

  getUsersWithRoles() {
    return this.http.get<Partial<User[]>>(
      this.baseUrl + 'admins/users-with-roles'
    );
  }

  updateUserRoles(username: string, roles: string[]) {
    return this.http.post(
      this.baseUrl + 'admins/edit-roles/' + username + '?roles=' + roles,
      {}
    );
  }
}
