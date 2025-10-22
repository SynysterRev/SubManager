import { Component, input } from '@angular/core';

@Component({
  selector: 'app-active-subscriptions-card',
  imports: [],
  templateUrl: './active-subscriptions-card.html',
  styleUrl: './active-subscriptions-card.scss'
})
export class ActiveSubscriptionsCard {
  activeSub = input.required<number>();
  totalSub = input.required<number>();
}
