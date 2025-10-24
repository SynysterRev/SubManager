import { Component, computed, DestroyRef, effect, inject, signal } from '@angular/core';
import { Header } from "../../../../core/layout/header/header";
import { SubscriptionTotalCard } from "../subscription-total-card/subscription-total-card";
import { ActiveSubscriptionsCard } from "../active-subscriptions-card/active-subscriptions-card";
import { SubscriptionCard } from "../subscription-card/subscription-card";
import { ModalService } from '../../../../core/services/modal';
import { AddSubscriptionModal } from "../add-subscription-modal/add-subscription-modal";
import { SubscriptionCreateDto, SubscriptionDto, SubscriptionFormData, SubscriptionUpdateDto } from '../../models/subscription.model';
import { SubscriptionService } from '../../services/subscription';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { DeleteSubscriptionModal } from "../delete-subscription-modal/delete-subscription-modal";
import { Category } from '../../models/category.model';
import { CategoryService } from '../../services/category';

@Component({
  selector: 'app-subscriptions-dashboard',
  imports: [Header, SubscriptionTotalCard, ActiveSubscriptionsCard, SubscriptionCard, AddSubscriptionModal, DeleteSubscriptionModal],
  templateUrl: './subscriptions-dashboard.html',
  styleUrl: './subscriptions-dashboard.scss'
})
export class SubscriptionsDashboard {

  private destroyRef = inject(DestroyRef);
  modalService = inject(ModalService);
  subService = inject(SubscriptionService);
  categoryService = inject(CategoryService);

  totalSubActive = computed(() => this.subscriptions().filter(s => s.isActive).length);

  subscriptions = signal<SubscriptionDto[]>([]);
  totalCostMonth = signal<number>(0);
  totalCostYear = signal<number>(0);
  categories = signal<Category[]>([]);

  isAddModalOpen = signal(false);
  isDeleteModalOpen = signal(false);
  isEditModalOpen = signal(false);

  constructor() {

    this.loadSubscriptions();

    effect(() => {
      this.isAddModalOpen.set(this.modalService.openModal() === 'addSubscription');
    })

    effect(() => {
      this.isEditModalOpen.set(this.modalService.openModal() === 'editSubscription');
    })

    effect(() => {
      this.isDeleteModalOpen.set(this.modalService.openModal() === 'deleteSubscription');
    })

    this.categoryService.getAllCategories()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(categories => {
        this.categories.set(categories);
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

    const totalRounded = parseFloat(total.toFixed(2));
    this.totalCostMonth.set(totalRounded);
    this.totalCostYear.set(parseFloat((totalRounded * 12).toFixed(2)));
  }

  handleSubscription(formData: SubscriptionFormData) {
    if (this.isEditModalOpen()) {

      const sub: SubscriptionDto = this.modalService.modalData()?.data;
      if (!sub) return;

      const updateDto: SubscriptionUpdateDto = formData;
      const previousList = this.subscriptions();

      this.subscriptions.update(list =>
        list.map(s => s.id === sub.id ? { ...s, ...formData } as SubscriptionDto : s)
      );

      this.recalculateTotals();

      this.modalService.closeModal();
      // check to reuse onSubscriptionToggled
      this.subService.updateSubscription(sub.id, updateDto)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: (updatedSub) => {
            this.loadSubscriptions();
          },
          error: (err) => {
            console.error('toggle failed: ', err);

            this.subscriptions.set(previousList);
            this.recalculateTotals();
            // need to optimize here and then and not reload everything each time
            this.loadSubscriptions();
          }
        });
    } else {
      // since it's already validate by the form
      const createDto: SubscriptionCreateDto = {
        name: formData.name!,
        categoryId: formData.categoryId,
        price: formData.price!,
        paymentDay: formData.paymentDay!
      };
      console.log(createDto);
      this.subService.createNewSubcription(createDto).subscribe({
        next: (newSub) => {
          this.subscriptions.update(list => [...list, newSub]);
          this.recalculateTotals();

          this.loadSubscriptions();
          this.modalService.closeModal();
        }
      });
    }
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

  onDeleteSubscription() {
    const sub: SubscriptionDto = this.modalService.modalData()?.data;
    if (!sub) return;
    this.subscriptions.update(list =>
      list.filter(s => s.id !== sub.id)
    );
    this.recalculateTotals();
    this.subService.deleteSubscription(sub.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.loadSubscriptions();
          this.modalService.closeModal();
        }
      })
  }
}
