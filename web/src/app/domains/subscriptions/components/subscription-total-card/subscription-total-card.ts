import { Component, input } from '@angular/core';
import { CurrencyPipe } from '../../pipes/currency-pipe';

@Component({
  selector: 'app-subscription-total-card',
  imports: [CurrencyPipe],
  templateUrl: './subscription-total-card.html',
  styleUrl: './subscription-total-card.scss'
})
export class SubscriptionTotalCard {
  monthlyCost = input.required<number>();
  yearCost = input.required<number>();
  currencyCode = input.required<string>();
}
