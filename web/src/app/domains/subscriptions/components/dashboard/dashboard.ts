import { Component } from '@angular/core';
import { Header } from "../../../../core/layout/header/header";
import { SubscriptionTotalCard } from "../subscription-total-card/subscription-total-card";
import { ActiveSubscriptionsCard } from "../active-subscriptions-card/active-subscriptions-card";

@Component({
  selector: 'app-dashboard',
  imports: [Header, SubscriptionTotalCard, ActiveSubscriptionsCard],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard {

}
