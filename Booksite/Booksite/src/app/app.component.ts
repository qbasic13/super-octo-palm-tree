import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { AuthService } from './services/auth.service';
import { EventBusService } from './services/event-bus.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'Booksite';
  isLoggedIn = false;
  eventBusSub?: Subscription;

  constructor(private authService: AuthService, private eventBusService: EventBusService) { }
  ngOnInit(): void {
    this.isLoggedIn = (localStorage.getItem("access") !== null);
    if (this.isLoggedIn) {
    }
    this.eventBusSub = this.eventBusService.on('logout', () => {
      this.logout();
    });
  }
  ngOnDestroy(): void {
    if (this.eventBusSub)
      this.eventBusSub.unsubscribe();
  }
  logout(): void {
    this.authService.signOut();
  }
}
