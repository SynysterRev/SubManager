import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { SubscriptionCreateDto, SubscriptionDto } from '../models/subscription.model';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SubscriptionService {
  private http = inject(HttpClient);

  createNewSubcription(sub: SubscriptionCreateDto): Observable<SubscriptionDto> {
    return this.http.post<SubscriptionDto>(
      `${environment.apiUrl}/me/subscriptions`,
      sub,
      { withCredentials: true }
    );
  }
}
