import { Component } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { AuthRes } from 'src/app/models/auth.model';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-verify-email',
  templateUrl: './verify-email.component.html',
  styleUrls: ['./verify-email.component.css']
})
export class VerifyEmailComponent {
  authRes: AuthRes = {
    isSuccess: false,
    message: "no request was made yet",
    accessToken: " "
  };
  hide: boolean = true;
  constructor(private authService: AuthService,
    private route: ActivatedRoute) {
    this.route.queryParamMap.subscribe((params) => {
      const email = params.get('email');
      if (!email || email.length < 3) {
        this.authRes.message = "no email address supplied";
        this.hide = false;
      } else {
        this.verifyEmail(email);
      }
    });
    
  }

  verifyEmail(email: string): void {
    var result = this.authService.verifyEmail(email).subscribe(
      (response) => {
        this.authRes = response as AuthRes;
        this.hide = false;
        return response;
      },
      (error) => {
        const res: AuthRes = {
          isSuccess: false,
          message: "request failed.",
          accessToken: " "
        };
        this.authRes = res;
        this.hide = false;
        return res;
      });
  }
}
