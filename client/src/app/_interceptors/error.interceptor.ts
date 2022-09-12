import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError, Observable, throwError } from 'rxjs';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error) => {
        if (error) {
          this.correctErrorStatusText(error);
          switch (error.status) {
            case 400:
            case 429:
              if (error.error.errors) {
                const modalStateErrors = [];
                for (const key in error.error.errors) {
                  if (error.error.errors) {
                    modalStateErrors.push(error.error.errors[key]);
                  }
                }
                throw modalStateErrors.flat();
              } else if (typeof error.error === 'object') {
                this.toastr.error(error.statusText, error.status);
              } else {
                this.toastr.error(error.error, error.status);
              }
              break;
            case 401:
              this.toastr.error(error.statusText, error.status);
              break;
            case 404:
              this.router.navigateByUrl('/not-found');
              break;
            case 500:
              const navigationExtras: NavigationExtras = {
                state: { error: error.error },
              };
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
    );
  }

  private correctErrorStatusText(error) {
    if (error.statusText !== 'OK') {
      return error.statusText;
    }

    switch (error.status) {
      case 500:
        return (error.statusText = error.statusText = 'Internal Server Error');
      case 400:
        return (error.statusText = error.statusText = 'Bad Request');
      case 401:
        return (error.statusText = error.statusText = 'Unauthorized');
      case 404:
        return (error.statusText = error.statusText = 'Not Found');
      case 429:
        return (error.statusText = error.statusText = 'Too Many Requests');
      default:
        break;
    }
  }
}
