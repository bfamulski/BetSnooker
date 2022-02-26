import { Component, HostListener } from '@angular/core';
import { Router } from '@angular/router';

import { AuthenticationService, SnookerFeedService } from './_services';
import { User, Event } from './_models';
import { SwPush } from '@angular/service-worker';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less']
})
export class AppComponent {
    currentUser: User;
    currentEvent: Event;

    eventName: string;

    readonly VAPID_PUBLIC_KEY = "BPD84WXKqL81yrFsmQtCRBrLJW8xp7H6mlazwu0ldX_VzbcW0u3HxkhtT7WGoXfbHnPRpfFTuAtyBCa-xoMEOxw";

    constructor(
        private router: Router,
        private authenticationService: AuthenticationService,
        private snookerFeedService: SnookerFeedService,
        private swPush: SwPush) {

        this.authenticationService.currentUser.subscribe(x => this.currentUser = x);
        this.snookerFeedService.getCurrentEvent(true).subscribe(event => {
            this.currentEvent = event;
            this.eventName = `${event.sponsor} ${event.name}`.trim();
        });
    }

    ngOnInit() {
        if (!this.swPush.isEnabled) {
            console.log('Notification is not enabled');
            return;
        }

        console.log('Notification is enabled')

        this.swPush.requestSubscription({
            serverPublicKey: this.VAPID_PUBLIC_KEY
        }).then(sub => console.log(JSON.stringify(sub)))
        .catch(err => console.log(err));
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
