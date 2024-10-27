import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError } from 'rxjs/operators';


@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private apiUrl = 'http://localhost:59552/api';

  constructor(private httpClient: HttpClient) { }

  getDetails() {
    return this.httpClient.get(`${this.apiUrl}/dashboard/details`)
      .pipe(
        catchError((error) => {
          console.error('Error in DashboardService:', error);
          throw error; // Re-throw the error to propagate it to the caller
        })
      );
  }
}
