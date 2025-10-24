import { Component, input } from '@angular/core';

@Component({
  selector: 'app-subscription-total-card',
  imports: [],
  templateUrl: './subscription-total-card.html',
  styleUrl: './subscription-total-card.scss'
})
export class SubscriptionTotalCard {
  monthlyCost = input.required<number>();
  yearCost = input.required<number>();
}
