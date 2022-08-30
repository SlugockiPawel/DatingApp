import {Component, OnInit} from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  registerMode = false;
  learnMoreMode = false;

  constructor() {
  }

  ngOnInit() {
  }

  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  learnMoreToggle() {
    this.learnMoreMode = !this.learnMoreMode;
  }

  cancelRegisterMode(event: boolean) {
    this.registerMode = event;
  }

  cancelLearnMore(event: boolean) {
    this.learnMoreMode = event;
  }
}
