import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router } from '@angular/router'; // Added CanActivate
import { AuthService } from './auth.service';
import { SnackbarService } from './snackbar.service';
import jwt_decode from 'jwt-decode';
import { GlobalConstants } from '../shared/global-constants';

@Injectable({
  providedIn: 'root'
})
export class RouteGuardService implements CanActivate { // Implemented CanActivate
  constructor(
    private auth: AuthService,
    private router: Router,
    private snackbarService: SnackbarService
  ) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    let expectedRoleArray = route.data; // Use 'const' for expectedRoleArray
    expectedRoleArray = expectedRoleArray.expectedRole;
    const token:any = localStorage.getItem('token');

    var tokenPayload:any;
    try{
      tokenPayload = jwt_decode(token);
    }
    catch(err){
      localStorage.clear();
      this.router.navigate(['/']);
    }

    let expectedRole = '';

    for(let i = 0;i<expectedRoleArray.length;i++){
      if(expectedRoleArray[i] == tokenPayload.role){
        expectedRole = tokenPayload.role;
      }
    }

    if(tokenPayload.role == 'user' || tokenPayload.role == 'admin'){
      if(this.auth.isAuthenticated() && tokenPayload.role == expectedRole){
        return true;
      }
      this.snackbarService.openSnackBar(GlobalConstants.unauthorized,GlobalConstants.error);
      this.router.navigate(['/car/dashboard']);
      return false;
    }
    else{
      this.router.navigate(['/']);
      localStorage.clear();
      return false;
    }



    // if (!token) {
    //   localStorage.clear();
    //   this.router.navigate(['/']);
    //   return false;
    // }

    // try {
    //   tokenPayload = jwt_decode(token);
    // } catch (err) {
    //   localStorage.clear();
    //   this.router.navigate(['/']);
    //   return false;
    // }

    // if (expectedRoleArray.includes(tokenPayload.role)) {
    //   if (tokenPayload.role === 'user' || tokenPayload.role === 'admin') {
    //     if (this.auth.isAuthenticated() && tokenPayload.role === expectedRoleArray[0]) {
    //       return true;
    //     } else {
    //       this.snackbarService.openSnackBar(GlobalConstants.unauthorized, GlobalConstants.error);
    //       this.router.navigate(['/car/dashboard']);
    //       return false;
    //     }
    //   } else {
    //     this.router.navigate(['/']);
    //     localStorage.clear();
    //     return false;
    //   }
    // } else {
    //   // Handle the case where the user's role does not match any expected roles
    //   this.router.navigate(['/']); // Redirect to the home page or an error page
    //   localStorage.clear();
    //   return false;
    // }
  }
}
