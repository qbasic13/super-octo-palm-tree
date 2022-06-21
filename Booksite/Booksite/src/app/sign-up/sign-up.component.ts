import { Component, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthReq, AuthRes, RegReq } from 'src/app/models/auth.model';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';
import { SnackNotifyComponent } from '../snack-notify/snack-notify.component';

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.css']
})
export class SignUpComponent {

  @ViewChild('signupForm') signupForm: any;
  signUpForm: FormGroup;
  email = new FormControl('', [Validators.required, Validators.email]);
  password = new FormControl('', [Validators.required]);
  confirmPassword = new FormControl('', [Validators.required]);
  phone = new FormControl('', [Validators.required, Validators.pattern('(([+][(]?[0-9]{1,3}[)]?)|([(]?[0-9]{4}[)]?))\s*[)]?[-\s\.]?[(]?[0-9]{1,3}[)]?([-\s\.]?[0-9]{3})([-\s\.]?[0-9]{3,4})')]);
  firstName = new FormControl('', [Validators.required]);
  lastName = new FormControl('', [Validators.maxLength(32)]);
  middleName = new FormControl('', [Validators.maxLength(32)]);
  hide = true;


  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snack: SnackNotifyComponent
  ) {
    this.signUpForm = this.formBuilder.group({
      email: this.email,
      password: this.password,
      confirmPassword: this.confirmPassword,
      phone: this.phone,
      firstName: this.firstName,
      lastName: this.lastName,
      middleName: this.middleName
    });
  }

  getErrorMessage(field: string): string | void {
    // @ts-ignore
    const classField = this[field];
    if (classField?.hasError('required')) {
      return 'You must enter a value';
    } else if (classField?.hasError('email')) {
      return 'Not a valid email';
    } else if (classField?.hasError('confirmPassword')) {
      return 'Passwords must match';
    } else if (classField?.hasError('phone')) {
      return 'Incorrect phone format';
    }
  }

  sendForm() {
    if (this.signUpForm.valid) {
      const formValue = this.signUpForm.value;
      const regReq: RegReq = {
        email: formValue.email,
        password: formValue.password,
        fingerprint: localStorage.getItem('fingerprint')!,
        firstName: formValue.firstName,
        lastName: formValue.lastName,
        middleName: formValue.middleName,
        phone: formValue.phone
      }
      this.authService.signUp(regReq).subscribe((response: AuthRes) => {
        if (response.isSuccess) {
          localStorage.setItem("access", response.accessToken);
          const jwtPayload = this.authService.parseJwt(response.accessToken);
          localStorage.setItem("email", jwtPayload.email);
          localStorage.setItem("role", jwtPayload.role);

          this.snack.openSnackBar(`Registered ${jwtPayload.email}`, 'Ok')

          this.router.navigate(['/']);
        } else {
          this.snack.openSnackBar(`Error: ${response.message}`, 'Ok');
        }
      });
    }
  }
}
