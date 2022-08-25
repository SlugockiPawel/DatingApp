import {Component, OnInit} from '@angular/core';
import {Photo} from '../../../_models/photo';
import {AdminService} from '../../../_services/admin.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css'],
})
export class PhotoManagementComponent implements OnInit {
  photosToApprove: Photo[];

  constructor(private readonly adminService: AdminService) {
  }

  ngOnInit() {
    this.getPhotosForModeration();
  }

  getPhotosForModeration() {
    this.adminService.getPhotosForModeration().subscribe((photos: Photo[]) => {
      this.photosToApprove = photos;
    });
  }

  approvePhoto(photoId: number) {
    this.adminService.approvePhoto(photoId).subscribe(() =>
      this.photosToApprove.splice(
        this.photosToApprove.findIndex(p => p.id === photoId),
        1
      )
    );
  }

  rejectPhoto(photoId: number) {
    this.adminService.rejectPhoto(photoId).subscribe(() =>
      this.photosToApprove.splice(
        this.photosToApprove.findIndex(p => p.id === photoId),
        1
      )
    );
  }
}
