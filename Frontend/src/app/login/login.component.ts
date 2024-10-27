import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { MatDialogRef } from '@angular/material/dialog';
import { GlobalConstants } from '../shared/global-constants';
import { tokenName } from '@angular/compiler';
import { SnackbarService } from '../services/snackbar.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  hide = true;
  loginForm:any = FormGroup; // Use definite assignment assertion
  responseMessage: any;


  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private userService: UserService,
    private snackbarService: SnackbarService,
    public dialogRef: MatDialogRef<LoginComponent>
  ) {}

  ngOnInit(): void {
    this.loginForm = this.formBuilder.group({
      email: [null, [Validators.required, Validators.pattern(GlobalConstants.emailRegex)]],
      password: [null, [Validators.required]]
    })
  }

  handleSubmit() {
    var formData = this.loginForm.value;
    var data = {
      email: formData.email,
      password: formData.password
    };

    this.userService.login(data).subscribe(
      (response: any) => {
        console.log('login')

        this.dialogRef.close();
        localStorage.setItem('token', response.token);
        this.router.navigate(['/car/dashboard']);
      },
      (error) => {
        if (error.error?.message) {
          this.responseMessage = error.error.message;
        } else {
          this.responseMessage = GlobalConstants.genericError;
        }
        this.snackbarService.openSnackBar(this.responseMessage, GlobalConstants.error);
      }
    );
  }

  



}
