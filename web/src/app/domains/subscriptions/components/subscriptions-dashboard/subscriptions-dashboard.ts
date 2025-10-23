import { Component, computed, DestroyRef, effect, inject, signal } from '@angular/core';
import { Header } from "../../../../core/layout/header/header";
import { SubscriptionTotalCard } from "../subscription-total-card/subscription-total-card";
import { ActiveSubscriptionsCard } from "../active-subscriptions-card/active-subscriptions-card";
import { SubscriptionCard } from "../subscription-card/subscription-card";
import { ModalService } from '../../../../core/services/modal';
import { AddSubscriptionModal } from "../add-subscription-modal/add-subscription-modal";
import { SubscriptionDto } from '../../models/subscription.model';
import { SubscriptionService } from '../../services/subscription';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-subscriptions-dashboard',
  imports: [Header, SubscriptionTotalCard, ActiveSubscriptionsCard, SubscriptionCard, AddSubscriptionModal],
  templateUrl: './subscriptions-dashboard.html',
  styleUrl: './subscriptions-dashboard.scss'
})
export class SubscriptionsDashboard {

  private destroyRef = inject(DestroyRef);
  modalService = inject(ModalService);
  subService = inject(SubscriptionService);

  totalSubActive = computed(() => this.subscriptions().filter(s => s.isActive).length);

  subscriptions = signal<SubscriptionDto[]>([]);
  totalCostMonth = signal<number>(0);
  totalCostYear = signal<number>(0);

  isModalOpen = signal(false);

  constructor() {

    this.loadSubscriptions();

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

  private loadSubscriptions() {
    this.subService.getSubscriptions()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(response => {
        this.subscriptions.set(response.items);
        this.totalCostMonth.set(response.totalCostMonth);
        this.totalCostYear.set(response.totalCostYear);
      });
  }

  private recalculateTotals() {
    const total = this.subscriptions()
      .filter(s => s.isActive)
      .reduce((sum, s) => sum + s.price, 0);
    this.totalCostMonth.set(total);
    this.totalCostYear.set(total * 12);
  }

  handleNewSubscription(newSub: SubscriptionDto) {
    this.subscriptions.update(list => [...list, newSub]);
    this.recalculateTotals();

    this.loadSubscriptions();
  }

  onSubscriptionToggled(update: SubscriptionDto) {
    const previousList = this.subscriptions();

    this.subscriptions.update(list => list.map(s => s.id == update.id ? update : s));
    this.recalculateTotals();

    this.subService.updateSubscription(update.id, {
      isActive: update.isActive
    })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (updatedSub) => {
          this.subscriptions.update(list =>
            list.map(s => s.id === updatedSub.id ? updatedSub : s)
          );
          this.recalculateTotals();

          this.loadSubscriptions();
        },
        error: (err) => {
          console.error('toggle failed: ', err);

          this.subscriptions.set(previousList);
          this.recalculateTotals();
          
          this.loadSubscriptions();
        }
      });
  }
}
