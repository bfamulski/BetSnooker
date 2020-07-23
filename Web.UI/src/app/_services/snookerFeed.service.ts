import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable, BehaviorSubject } from 'rxjs';

import { environment } from '../../environments/environment';
import { Match, RoundInfo, Event } from '../_models';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class SnookerFeedService {

    constructor(private http: HttpClient) { }

    getCurrentEvent(): Observable<Event> {
        const currentEvent = JSON.parse(localStorage.getItem('currentEvent'));
        if (currentEvent) {
            const currentEventSubject = new BehaviorSubject<Event>(currentEvent);
            return currentEventSubject.asObservable();
        }

        return this.http.get<Event>(`${environment.apiUrl}/snookerFeed/events/current`)
            .pipe(map(event => {
                localStorage.setItem('currentEvent', JSON.stringify(event));
                return event;
            }));
    }

    getEventMatches(): Observable<Match[]> {
        return this.http.get<Match[]>(`${environment.apiUrl}/snookerFeed/matches/all`);
    }

    getCurrentRoundInfo(): Observable<RoundInfo> {
        return this.http.get<RoundInfo>(`${environment.apiUrl}/snookerFeed/rounds/current`);
    }

    getEventRounds(): Observable<RoundInfo[]> {
        return this.http.get<RoundInfo[]>(`${environment.apiUrl}/snookerFeed/rounds/all`);
    }
}
