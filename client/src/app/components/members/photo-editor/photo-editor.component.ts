import {Component, Input, OnInit} from '@angular/core';
import {Member} from "../../../_models/member";
import {FileUploader} from "ng2-file-upload";
import {environment} from "../../../../environments/environment";
import {User} from "../../../_models/user";
import {AccountService} from "../../../_services/account.service";
import {take} from "rxjs";
import {Photo} from "../../../_models/photo";
import {MembersService} from "../../../_services/members.service";

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() member: Member;
  uploader: FileUploader;
  hasBaseDropzoneOver: boolean = false;
  baseUrl = environment.apiUrl;
  user: User;

  constructor(private accountService: AccountService, private membersService: MembersService) {
    accountService.currentUser$.pipe(take<User>(1)).subscribe({
      next: user => this.user = user
    });
  }

  ngOnInit(): void {
    this.initializeUploader();
  }

  setMainPhoto(photo: Photo) {
    this.membersService.setMainPhoto(photo.id).subscribe(next => {
      this.user.mainPhotoUrl = photo.url;
      this.accountService.setCurrentUser(this.user);
      this.member.photoUrl = photo.url;
      this.member.photos.forEach(p => {
          if (p.isMain) p.isMain = false;
          if(p.id === photo.id) p.isMain = true;
        }
      );
    });
  }

  fileOverBase(e: any) {
    this.hasBaseDropzoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.user.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
      this.uploader.onSuccessItem = (item, response, status, headers) => {
        const photo = JSON.parse(response);
        this.member.photos.push(photo);
      }
    }
  }

}
