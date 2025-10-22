import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { Header } from "../../../../core/layout/header/header";
import { SubscriptionTotalCard } from "../subscription-total-card/subscription-total-card";
import { ActiveSubscriptionsCard } from "../active-subscriptions-card/active-subscriptions-card";
import { SubscriptionCard } from "../subscription-card/subscription-card";
import { ModalService } from '../../../../core/services/modal';
import { AddSubscriptionModal } from "../add-subscription-modal/add-subscription-modal";
import { Subscription } from 'rxjs';
import { SubscriptionDto } from '../../models/subscription.model';

@Component({
  selector: 'app-subscriptions-dashboard',
  imports: [Header, SubscriptionTotalCard, ActiveSubscriptionsCard, SubscriptionCard, AddSubscriptionModal],
  templateUrl: './subscriptions-dashboard.html',
  styleUrl: './subscriptions-dashboard.scss'
})
export class SubscriptionsDashboard implements OnInit, OnDestroy {

  modalService = inject(ModalService);
  isModalOpen: boolean = false;
  private sub?: Subscription;

  ngOnInit() {
    this.modalService.openModal$.subscribe(() => {
      this.isModalOpen = true;
    });
  }

  ngOnDestroy() {
    this.sub?.unsubscribe();
  }

  handleNewSubscription(data: SubscriptionDto) {

  }
}
