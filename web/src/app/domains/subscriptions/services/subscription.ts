import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { SubscriptionCreateDto, SubscriptionDto, SubscriptionUpdateDto } from '../models/subscription.model';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { PaginatedResponse } from '../../../shared/models/paginated-response.model';

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

  getSubscriptions(): Observable<PaginatedResponse> {
    return this.http.get<PaginatedResponse>(
      `${environment.apiUrl}/me/subscriptions`,
      { withCredentials: true }
    );
  }

  updateSubscription(id: number, sub: SubscriptionUpdateDto): Observable<SubscriptionDto> {
    return this.http.put<SubscriptionDto>(
      `${environment.apiUrl}/me/subscriptions/${id}`,
      sub,
      { withCredentials: true }
    );
  }

  deleteSubscription(id: number): Observable<void> {
    return this.http.delete<void>(
      `${environment.apiUrl}/me/subscriptions/${id}`,
      { withCredentials: true }
    );
  }
}
