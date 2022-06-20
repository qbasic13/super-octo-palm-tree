import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-snack-notify',
  templateUrl: './snack-notify.component.html',
  styleUrls: ['./snack-notify.component.css']
})
export class SnackNotifyComponent {
  durationInSeconds = 3;
  constructor(private _snackBar: MatSnackBar) { }

  openSnackBar(message: string, action: string) {
    this._snackBar.open(message, action, { duration: this.durationInSeconds * 1000 });
  }
}
