import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { AuthenticationService, SnookerFeedService } from './_services';
import { User, Event } from './_models';

@Component({
    selector: 'app-root',
    templateUrl: 'app.component.html',
    styleUrls: ['app.component.less']
})
export class AppComponent {
    currentUser: User;
    currentEvent: Event;

    constructor(
        private router: Router,
        private authenticationService: AuthenticationService,
        private snookerFeedService: SnookerFeedService) {
        this.authenticationService.currentUser.subscribe(x => this.currentUser = x);
        this.snookerFeedService.getCurrentEvent().subscribe(event => this.currentEvent = event);
    }

    logout() {
        this.authenticationService.logout();
        this.router.navigate(['/login']);
    }
}
