import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { environment } from '../../environments/environment';
import { RoundBets } from '../_models/bet';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class BetsService {

    private httpOptions = {
        headers: new HttpHeaders({ 'Content-Type': 'application/json' })
      };

    constructor(private http: HttpClient) { }

    getBets(roundId: number): Observable<RoundBets> {
        return this.http.get<RoundBets>(`${environment.apiUrl}/bets/${roundId}`);
    }

    getAllBets(roundId: number): Observable<RoundBets[]> {
        return this.http.get<RoundBets[]>(`${environment.apiUrl}/bets/all/${roundId}`);
    }

    submitBets(bets: RoundBets) {
        const body = JSON.stringify(bets);
        return this.http.post(`${environment.apiUrl}/bets`, body, this.httpOptions);
    }
}
