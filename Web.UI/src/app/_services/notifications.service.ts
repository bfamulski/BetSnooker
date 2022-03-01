import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class NotificationsService {

    constructor(private http: HttpClient) { }

    addSubscriber(sub: PushSubscription) {
        return this.http.post(`${environment.apiUrl}/notifications`, sub);
    }
}
