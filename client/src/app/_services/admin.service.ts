import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {environment} from '../../environments/environment';
import {Photo} from '../_models/photo';
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

  getPhotosForModeration() {
    return this.http.get<Photo[]>(this.baseUrl + 'admins/photos-to-moderate');
  }

  approvePhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'admins/approve-photo/' + photoId, {});
  }

  rejectPhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'admins/reject-photo/' + photoId);
  }
}
