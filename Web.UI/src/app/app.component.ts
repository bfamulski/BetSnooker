import { Component, HostListener } from '@angular/core';
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
    currentEventFullName: string;

    constructor(
        private router: Router,
        private authenticationService: AuthenticationService,
        private snookerFeedService: SnookerFeedService) {

        this.authenticationService.currentUser.subscribe(x => this.currentUser = x);
        this.snookerFeedService.getCurrentEvent().subscribe(event => {
            this.currentEvent = event;
            this.currentEventFullName = `${event.sponsor} ${event.name}`.trim();
        });
    }

    logout() {
        this.authenticationService.logout();
        this.router.navigate(['/login']);
    }

    showDropdown() {
        document.getElementById('userDropdown').classList.toggle('show');
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
