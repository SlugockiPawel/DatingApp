import {Component, OnInit} from '@angular/core';
import {BsModalRef, BsModalService, ModalOptions} from 'ngx-bootstrap/modal';
import {User} from '../../../_models/user';
import {AdminService} from '../../../_services/admin.service';
import {RolesModalComponent} from '../../../modals/roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css'],
})
export class UserManagementComponent implements OnInit {
  users: Partial<User[]>;
  modalRef?: BsModalRef;

  constructor(
    private readonly adminService: AdminService,
    private readonly modalService: BsModalService
  ) {
  }

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    return this.adminService.getUsersWithRoles().subscribe((users) => {
      this.users = users;
    });
  }

  openRolesModal() {
    const initialState: ModalOptions = {
      initialState: {
        list: [
          'Open a modal with component',
          'Pass your data',
          'Do something else',
          '...',
        ],
        title: 'Modal with component',
      },
    };
    this.modalRef = this.modalService.show(RolesModalComponent, initialState);
    this.modalRef.content.closeBtnName = 'Close';
  }
}
