import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { Member } from '../_models/member';

const httpOptions = {
  headers: new HttpHeaders({
    Authorization: 'Bearer ' + JSON.parse(localStorage.getItem('user'))?.token
  })
}

@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getMembers(): Observable<Member[]>{
    return this.http.get<Member[]>(this.baseUrl + 'users', httpOptions);
  }

  getMember(username: string): Observable<Member> {
    return this.http.get<Member>(this.baseUrl + 'users/' + username, httpOptions);
  }
}
