import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { environment } from '../../environments/environment';
import { Event } from '../_models';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AdminService {

  private httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    };

  constructor(private http: HttpClient) { }

  getCurrentEvent(): Observable<Event> {
    return this.http.get<Event>(`${environment.apiUrl}/admin/currentEvent`, this.httpOptions);
  }

  setCurrentEvent(eventId: number) {
    const body = JSON.stringify(eventId);
    this.http.post(`${environment.apiUrl}/admin/currentEvent`, body, this.httpOptions);
  }

  setStartRound(roundId: number) {
    const body = JSON.stringify(roundId);
    this.http.post(`${environment.apiUrl}/admin/startRound`, body, this.httpOptions);
  }
}
