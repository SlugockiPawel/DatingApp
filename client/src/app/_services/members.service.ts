import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { Member } from '../_models/member';


@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) { }

  }

  getMember(username: string): Observable<Member> {
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member);
  }
}
