import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { SnackbarService } from '../services/snackbar.service';
import { MatDialogRef } from '@angular/material/dialog';
import { GlobalConstants } from '../shared/global-constants';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.scss']
})
export class SignupComponent implements OnInit {
  passwordVisible = false;
  confirmPasswordVisible = false;
  signupForm!: FormGroup; // Use definite assignment assertion

  responseMessage: any;

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private userService: UserService,
    private snackbarService: SnackbarService,
    public dialogRef: MatDialogRef<SignupComponent>
  ) {}

  ngOnInit(): void {
    this.signupForm = this.formBuilder.group({
      name: [null, [Validators.required]],
      email: [null, [Validators.required, Validators.email]],
      contactNumber: [null, [Validators.required, Validators.pattern('^[0-9]+$')]],
      password: [null, [Validators.required, Validators.minLength(6)]],
      confirmPassword: [null, [Validators.required]]
    });
  }

  togglePasswordVisibility() {
    this.passwordVisible = !this.passwordVisible;
  }

  toggleConfirmPasswordVisibility() {
    this.confirmPasswordVisible = !this.confirmPasswordVisible;
  }

  validateSubmit() {
    if (this.signupForm.controls['password'].value === this.signupForm.controls['confirmPassword'].value) {
      // Passwords match
      return true;
    } else {
      // Passwords do not match
      return false;
    }
  }

  handleSubmit() {
    // Extract data from the form
    const formData = this.signupForm.value;

    // Create a data object with the extracted form values
    const data = {
      name: formData.name,
      email: formData.email,
      contactNumber: formData.contactNumber,
      password: formData.password
    };

    // Call your signup method with the data object
    this.userService.signup(data).subscribe(
      (response: any) => {
        // Handle a successful signup response
        this.dialogRef.close();
        this.responseMessage = response?.message;
        this.snackbarService.openSnackBar(this.responseMessage, '');
        this.router.navigate(['/']);
      },
      (error) => {
        // Handle signup error
        if (error.error?.message) {
          this.responseMessage = error.error?.message;
        } else {
          this.responseMessage = GlobalConstants.genericError;
        }
        this.snackbarService.openSnackBar(this.responseMessage, GlobalConstants.error);
      }
    );
  }
}
