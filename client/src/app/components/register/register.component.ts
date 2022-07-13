import {ToastrService} from 'ngx-toastr';
import {AccountService} from './../../_services/account.service';
import {Component, Input, OnInit, Output, EventEmitter} from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  registerForm: FormGroup;

  constructor(private accountService: AccountService, private toastr: ToastrService) {
  }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.registerForm = new FormGroup({
      name: new FormControl('', Validators.required),
      password: new FormControl('',
        [Validators.required, Validators.minLength(6),
                        Validators.maxLength(20)]),
      confirmPassword: new FormControl('', Validators.required),
    });
  }

  register() {
    console.log(this.registerForm.value);

    // this.accountService.registerUser(this.model).subscribe({
    //   next:  (response) => {
    //     console.log(response);
    //     this.cancel();
    //   },
    //   error: (error) => {
    //     console.log(error),
    //     this.toastr.error(error.error);
    //   }
    // });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

}
