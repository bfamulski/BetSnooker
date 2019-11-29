import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { Match, RoundInfo } from '../_models';

@Injectable({ providedIn: 'root' })
export class SnookerFeedService {
    constructor(private http: HttpClient) { }

    getEvents(): Observable<Event[]> {
        return this.http.get<Event[]>(`${environment.apiUrl}/snookerFeed/events/${environment.season}`);
    }

    getRoundMatches(roundId: number): Observable<Match[]> {
        return this.http.get<Match[]>(`${environment.apiUrl}/snookerFeed/matches/${roundId}`);
    }

    getRoundInfo(roundId: number): Observable<RoundInfo> {
        return this.http.get<RoundInfo>(`${environment.apiUrl}/snookerFeed/round/${roundId}`);
    }
}
