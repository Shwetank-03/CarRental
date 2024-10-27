import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog'; // Fixed import statement
import { SignupComponent } from '../signup/signup.component'; // Fixed import statement
import { LoginComponent } from '../login/login.component';
import { UserService } from '../services/user.service';
import { Route, Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  constructor(private dialog: MatDialog,
    private userServices:UserService,
    private router:Router) {}

    ngOnInit(): void {
      this.userServices.checkToken().subscribe(
        (response: any) => {
          this.router.navigate(['/car/dashboard']);
        },
        (error: any) => {
          console.log(error);
        }
      );
    }



  handleSignupAction() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.width = "550px";
    this.dialog.open(SignupComponent,dialogConfig);
  }

  handleLoginAction() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.width = "550px";
    this.dialog.open(LoginComponent,dialogConfig);
  }
}
