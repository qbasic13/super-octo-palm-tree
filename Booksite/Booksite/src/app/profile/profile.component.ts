import { Component, OnInit } from '@angular/core';
import { Profile } from '../models/profile.model';
import { ProfileService } from '../services/profile.service';
import { SnackNotifyComponent } from '../snack-notify/snack-notify.component';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent {
  profile: any;
  isLoading: boolean = true;
  isError: boolean = false;
  constructor(private profileService: ProfileService,
    private snack: SnackNotifyComponent) {
    this.isLoading = true;
    this.isError = false;

    this.profileService.getProfileDetails('').subscribe(
      res => {
        if (res.isSuccess) {
          this.profile = res.profile;
          this.isLoading = false;
        } else {
          snack.openSnackBar(res.message ?? res.status, 'Ok');
          this.isError = true;
          this.isLoading = false;
        }
      },
      error => {
        snack.openSnackBar('Unknown error occured', 'Ok');
        this.isError = true;
        this.isLoading = false;
      }
    )
  }

}
