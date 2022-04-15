import { Component, HostListener } from '@angular/core';
import { Router } from '@angular/router';
import { SwPush, SwUpdate } from '@angular/service-worker';

import { AuthenticationService, SnookerFeedService, NotificationsService } from './_services';
import { User, Event } from './_models';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less']
})
export class AppComponent {
    currentUser: User;
    currentEvent: Event;

    eventName: string;

    constructor(
        private router: Router,
        private authenticationService: AuthenticationService,
        private snookerFeedService: SnookerFeedService,
        private swUpdate: SwUpdate,
        private swPush: SwPush,
        private notificationsService: NotificationsService) {

        if (this.swUpdate.isEnabled) {
            this.swUpdate.versionUpdates.subscribe(evt => {
                console.log(evt);
                switch (evt.type) {
                    case 'VERSION_DETECTED':
                        if (confirm('New version of BetSnooker available. Reload?')) {
                            location.reload();
                        }
                        break;
                    case 'VERSION_READY':
                        break;
                    case 'VERSION_INSTALLATION_FAILED':
                        console.log(`Failed to install app version '${evt.version.hash}': ${evt.error}`);
                        break;
                }
            });
        }

        this.swPush.requestSubscription({ serverPublicKey: environment.vapidPublicKey }).then(sub => {
            this.notificationsService.addSubscriber(sub).subscribe();
        }).catch(err => console.error("Could not subscribe to notifications", err));

        this.authenticationService.currentUser.subscribe(x => this.currentUser = x);
        this.snookerFeedService.getCurrentEvent(true).subscribe(event => {
            this.currentEvent = event;
            this.eventName = `${event.sponsor} ${event.name}`.trim();
        });
    }

    logout() {
        this.authenticationService.logout();
        this.router.navigate(['/login']);
    }

    showDropdown() {
        document.getElementById('userDropdown')!.classList.toggle('show');
    }

    // Close the dropdown if the user clicks outside of it
    @HostListener('document:click', ['$event'])
    onDocumentClick(event: MouseEvent) {
        if (!(event.target === document.getElementById('dropdownbtn'))) {
            const dropdowns = document.getElementsByClassName('dropdown-content');
            let i: number;
            for (i = 0; i < dropdowns.length; i++) {
                const openDropdown = dropdowns[i];
                if (openDropdown.classList.contains('show')) {
                    openDropdown.classList.remove('show');
                }
            }
        }
    }
}
