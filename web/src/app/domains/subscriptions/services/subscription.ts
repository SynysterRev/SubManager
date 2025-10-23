import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { SubscriptionCreateDto, SubscriptionDto } from '../models/subscription.model';
import { Observable, tap } from 'rxjs';
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
      { withCredentials: true });
  }
}
