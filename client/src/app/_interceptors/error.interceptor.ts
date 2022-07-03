import { ToastrService } from 'ngx-toastr';
import { NavigationExtras, Router } from '@angular/router';
import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(error => {
        if(error){
          this.correctErrorStatusText(error);
          switch(error.status) {
            case 400:
              if(error.error.errors) {
                const modalStateErrors = [];
                for(const key in error.error.errors) {
                  if(error.error.errors) {
                    modalStateErrors.push(error.error.errors[key]);
                  }
                }
                throw modalStateErrors.flat();
              }else {
                this.toastr.error(error.statusText, error.status);
              }
              break;
            case 401:
              this.toastr.error(error.statusText, error.status);
              break;
            case 404:
              this.router.navigateByUrl('/not-found');
              break;
            case 500:
              const navigationExtras: NavigationExtras = {state: {error: error.error}};
              this.router.navigateByUrl('/server-error', navigationExtras);
              break;
            default:
              this.toastr.error('Something unexpected happened');
              console.log(error);
              break;
          }
        }
        return throwError(error);
      })
    )
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


