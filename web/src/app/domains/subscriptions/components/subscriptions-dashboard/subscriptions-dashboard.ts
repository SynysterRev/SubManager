import { Component } from '@angular/core';
import { Header } from "../../../../core/layout/header/header";
import { SubscriptionTotalCard } from "../subscription-total-card/subscription-total-card";
import { ActiveSubscriptionsCard } from "../active-subscriptions-card/active-subscriptions-card";

@Component({
  selector: 'app-subscriptions-dashboard',
  imports: [Header, SubscriptionTotalCard, ActiveSubscriptionsCard],
  templateUrl: './subscriptions-dashboard.html',
  styleUrl: './subscriptions-dashboard.scss'
})
export class Dashboard {

}
