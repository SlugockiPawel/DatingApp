import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Injectable({
  providedIn: 'root',
})
export class ConfirmService {
  bsModalRef: BsModalRef;

  constructor(private readonly modalService: BsModalService) {}

  confirm(
    title = 'Confirmation',
    message = 'Are you sure?',
    btnOkText = 'Ok',
    btnCancelTxt = 'Cancel'
  ) {
    const config = {
      initialState: {
        title,
        message,
        btnOkText,
        btnCancelTxt,
      },
    };

    this.bsModalRef = this.modalService.show('Confirm', config);
  }
}
