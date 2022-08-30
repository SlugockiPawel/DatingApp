import {Component, EventEmitter, OnInit, Output} from '@angular/core';

@Component({
  selector: 'app-learn-more',
  templateUrl: './learn-more.component.html',
  styleUrls: ['./learn-more.component.css'],
})
export class LearnMoreComponent implements OnInit {
  @Output() cancelLearnMore: EventEmitter<boolean> = new EventEmitter();

  constructor() {
  }

  ngOnInit() {
  }

  cancel() {
    this.cancelLearnMore.emit(false);
  }
}
