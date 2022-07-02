import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-test-errors',
  templateUrl: './test-errors.component.html',
  styleUrls: ['./test-errors.component.css']
})
export class TestErrorsComponent implements OnInit {
  baseUrl = 'https://localhost:5001/api/';

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
  }

  get404Error() {
    this.http.get(this.baseUrl + 'buggy/not-found').subscribe({
      next: (response) => console.log(response),
      error: (error) => {
        this.correctErrorStatusText(error);
        console.log(error);
      },
    });
  }

  get400Error() {
    this.http.get(this.baseUrl + 'buggy/bad-request').subscribe({
      next: (response) => console.log(response),
      error: (error) => {
        this.correctErrorStatusText(error);
        console.log(error);
      },
    });
  }

  get500Error() {
    this.http.get(this.baseUrl + 'buggy/server-error').subscribe({
      next: (response) => console.log(response),
      error: (error) => {
        this.correctErrorStatusText(error);
        console.log(error);
      },
    });
  }

  get401Error() {
    this.http.get(this.baseUrl + 'Buggy/auth').subscribe({
      next: (response) => console.log(response),
      error: (error) => {
        this.correctErrorStatusText(error);
        console.log(error);
      },
    });
  }

  get400ValidationError() {
    this.http.post(this.baseUrl + 'account/register', {}).subscribe({
      next: (response) => console.log(response),
      error: (error) => {
        this.correctErrorStatusText(error);
        console.log(error);
      },
    });
  }

  private correctErrorStatusText(error) {
    if(error.statusText !== 'OK') {
      return error.statusText;
    }

    switch (error.status) {
      case 500:
        return error.statusText = error.statusText = "Internal Server Error";
      case 400:
        return error.statusText = error.statusText = "Bad Request";
      case 401:
        return error.statusText = error.statusText = "Unauthorized";
      case 404:
        return error.statusText = error.statusText = "Not Found";
      default:
      break;
    }
  }

}
