import { AccountService } from './../../_services/account.service';
import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  model: any = {};

  constructor(private accountService: AccountService) { }

  ngOnInit(): void {
  }

  register() {
    this.accountService.registerUser(this.model).subscribe({
      next:  (response) => {
        console.log(response);
        this.cancel();
      },
      error: (error: Error) => console.log(error),
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

}
