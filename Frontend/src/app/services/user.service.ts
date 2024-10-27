import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';


@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'http://localhost:59552/api'; // Update this URL as needed
  url: string;

  constructor(private httpClient: HttpClient) {
    this.url = this.apiUrl; // Assign the apiUrl to the 'url' property
  }

  signup(data: any) {
    return this.httpClient.post(this.url + "/user/signup", data, {
      headers: new HttpHeaders().set('Content-Type', 'application/json')
    })
  }

  login(data: any) {
    return this.httpClient.post(this.url + "/user/login", data, {
      headers: new HttpHeaders().set('Content-Type', 'application/json')
    })
  }

  checkToken(){
    return this.httpClient.get(this.url+"/user/checkToken");
  }

  changePassword(data: any) {
    return this.httpClient.post(`${this.url}/user/changePassword`, data, {
      headers: new HttpHeaders().set('Content-Type', 'application/json')
    });
  }
  getUsers() {
    return this.httpClient.get(this.url + "/user/getAllUser");
  }

  update(data: any) {
    return this.httpClient.post(this.url + "/user/updateUserStatus", data, {
      headers: new HttpHeaders().set('Content-Type', 'application/json')
    });
  }


}

