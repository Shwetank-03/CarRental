import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  update(data: { id: any; name: any; }) {
    throw new Error("Method not implemented.");
  }
  add(data: { name: any; }) {
    throw new Error("Method not implemented.");
  }
  url = 'http://localhost:59552/api'; // Replaced with the new URL

  constructor(private httpClient: HttpClient) {}

  addNewCategory(data: any) {
    return this.httpClient.post(`${this.url}/category/addNewCategory`, data, {
      headers: new HttpHeaders().set('Content-Type', 'application/json'),
    });
  }

  updateCategory(data: any) {
    return this.httpClient.post(`${this.url}/category/updateCategory`, data, {
      headers: new HttpHeaders().set('Content-Type', 'application/json'),
    });
  }

  getAllCategory(){
    return this.httpClient.get(this.url+"/category/getAllCategory");
  }

  getFilteredCategorys() {
    return this.httpClient.get(this.url + "/category/get?filterValue=true");
  }

}

