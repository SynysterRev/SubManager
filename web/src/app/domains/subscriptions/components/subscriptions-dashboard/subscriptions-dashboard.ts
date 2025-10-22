import { Component, effect, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { Header } from "../../../../core/layout/header/header";
import { SubscriptionTotalCard } from "../subscription-total-card/subscription-total-card";
import { ActiveSubscriptionsCard } from "../active-subscriptions-card/active-subscriptions-card";
import { SubscriptionCard } from "../subscription-card/subscription-card";
import { ModalService } from '../../../../core/services/modal';
import { AddSubscriptionModal } from "../add-subscription-modal/add-subscription-modal";
import { SubscriptionDto } from '../../models/subscription.model';

@Component({
  selector: 'app-subscriptions-dashboard',
  imports: [Header, SubscriptionTotalCard, ActiveSubscriptionsCard, SubscriptionCard, AddSubscriptionModal],
  templateUrl: './subscriptions-dashboard.html',
  styleUrl: './subscriptions-dashboard.scss'
})
export class SubscriptionsDashboard {

  modalService = inject(ModalService);

  subscriptions = signal<SubscriptionDto[]>([]);

  isModalOpen = signal(false);

  constructor() {
    effect(() => {
      this.isModalOpen.set(this.modalService.openModal() === 'addSubscription');
    })

    effect(() => {
      const data = this.modalService.modalData();
      if (data?.modal === 'addSubscription' && data.data) {
        this.handleNewSubscription(data.data);
      }
    });
  }

  handleNewSubscription(newSub: SubscriptionDto) {
    this.subscriptions.update(list => [...list, newSub]);
  }
}
