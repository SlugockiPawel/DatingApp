import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {map, Observable, of, take} from 'rxjs';
import {environment} from '../../environments/environment';
import {Member} from '../_models/member';
import {User} from '../_models/user';
import {UserParams} from '../_models/userParams';
import {AccountService} from './account.service';
import {getPaginatedResult, getPaginationHeaders} from './paginationHelper';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
  user: User;

  constructor(
    private http: HttpClient,
    private accountService: AccountService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user: User) => {
        this.user = user;
        this._userParams = new UserParams(user);
      },
    });
  }

  private _userParams: UserParams;

  get userParams() {
    return this._userParams;
  }

  set userParams(value: UserParams) {
    this._userParams = value;
  }

  resetUserParams() {
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }

  getMembers(userParams: UserParams) {
    // get data from cache
    const response = this.memberCache.get(Object.values(userParams).join('-'));
    if (response) {
      return of(response);
    }

    let params = getPaginationHeaders(
      userParams.pageNumber,
      userParams.pageSize
    );

    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    // put data in cache
    return getPaginatedResult<Member[]>(
      this.baseUrl + 'users',
      params,
      this.http
    ).pipe(
      map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      })
    );
  }

  getMember(userName: string): Observable<Member> {
    // get member out of memberCache
    const member = [...this.memberCache.values()]
      .reduce((arr, el) => arr.concat(el.result), [])
      .find((member: Member) => member.userName === userName);

    if (member) {
      return of(member);
    }

    return this.http.get<Member>(this.baseUrl + 'users/' + userName);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    );
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  addLike(userName: string) {
    return this.http.post(this.baseUrl + 'likes/' + userName, {});
  }

  removeLike(userName: string) {
    return this.http.delete(this.baseUrl + 'likes/' + userName, {});
  }

  getLikes(predicate: string, pageNumber: number, pageSize: number) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);

    return getPaginatedResult<Partial<Member[]>>(
      this.baseUrl + 'likes',
      params,
      this.http
    );
  }
}
