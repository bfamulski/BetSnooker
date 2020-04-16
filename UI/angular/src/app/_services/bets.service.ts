import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { environment } from '../../environments/environment';
import { RoundBets, EventBets } from '../_models/bet';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class BetsService {

    private httpOptions = {
        headers: new HttpHeaders({ 'Content-Type': 'application/json' })
      };

    constructor(private http: HttpClient) { }

    getUserBets(): Observable<RoundBets> {
        return this.http.get<RoundBets>(`${environment.apiUrl}/bets`);
    }

    getAllBets(): Observable<RoundBets[]> {
        return this.http.get<RoundBets[]>(`${environment.apiUrl}/bets/all/old`);
    }

    getEventBets(): Observable<EventBets[]> {
        return this.http.get<EventBets[]>(`${environment.apiUrl}/bets/all`);
    }

    submitBets(bets: RoundBets) {
        const body = JSON.stringify(bets);
        return this.http.post(`${environment.apiUrl}/bets`, body , this.httpOptions);
    }
}
