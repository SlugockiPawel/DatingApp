import {map, Observable, of} from 'rxjs';
import {HttpClient} from '@angular/common/http';
import {environment} from './../../environments/environment';
import {Injectable} from '@angular/core';
import {Member} from '../_models/member';


@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl: string = environment.apiUrl;
  members: Member[] = [];

  constructor(private http: HttpClient) {
  }

  getMembers(): Observable<Member[]> {
    if (this.members.length > 0) return of<Member[]>(this.members);
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map(members => {
        this.members = members;
        return members;
      })
    );
  }

  getMember(username: string): Observable<Member> {
    const member: Member = this.members.find(m => m.name === username);
    if (member !== undefined) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member)
      .pipe(
        map(() => {
          const index = this.members.indexOf(member);
          this.members[index] = member;
        })
      );
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + "users/set-main-photo/" + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }
}
