import { Component, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthReq, AuthRes } from 'src/app/models/auth.model';
import { AuthService } from 'src/app/services/auth.service'; 
import { Router } from '@angular/router';
import { SnackNotifyComponent } from '../snack-notify/snack-notify.component';

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.css']
})
export class SignInComponent {
  @ViewChild('signinForm') signinForm: any;
  signInForm: FormGroup;
  email = new FormControl('', [Validators.required, Validators.email]);
  password = new FormControl('', [Validators.required]);
  hide = true;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snack: SnackNotifyComponent
  ) {
    this.signInForm = this.formBuilder.group({
      email: this.email,
      password: this.password,
    });
  }

  getErrorMessage(field: string): string | void {
    // @ts-ignore
    const classField = this[field];
    if (classField?.hasError('required')) {
      return 'You must enter a value';
    } else if (classField?.hasError('email')) {
      return 'Not a valid email';
    }
  }

  sendForm() {
    if (this.signInForm.valid) {
      const formValue = this.signInForm.value;
      const authReq: AuthReq = {
        email : formValue.email,
        password : formValue.password,
        fingerprint : localStorage.getItem('fingerprint')!
      }
      this.authService.signIn(authReq).subscribe((response: AuthRes) => {
          if (response.isSuccess) {
            localStorage.setItem('access', response.accessToken);
            const jwtPayload = this.authService.parseJwt(response.accessToken);
            localStorage.setItem('email', jwtPayload.email);
            localStorage.setItem('role', jwtPayload.role);

            this.snack.openSnackBar(`Signed in as ${jwtPayload.email}`, 'Ok')

            this.router.navigate(['/']);
          } else {
            this.snack.openSnackBar(`Error: ${response.message}`, 'Ok')
        }
      });
    }
  }

}
