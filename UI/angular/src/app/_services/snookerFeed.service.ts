import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { Match, RoundInfo } from '../_models';

@Injectable({ providedIn: 'root' })
export class SnookerFeedService {
    constructor(private http: HttpClient) { }

    getCurrentEvent(): Observable<Event> {
        return this.http.get<Event>(`${environment.apiUrl}/snookerFeed/event/current`);
    }

    getEventMatches(): Observable<Match[]> {
        return this.http.get<Match[]>(`${environment.apiUrl}/snookerFeed/matches/all`);
    }

    getCurrentRoundInfo(): Observable<RoundInfo> {
        return this.http.get<RoundInfo>(`${environment.apiUrl}/snookerFeed/round/current`);
    }
}
