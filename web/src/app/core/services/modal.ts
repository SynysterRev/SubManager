import { Injectable, signal } from '@angular/core';
import { Subject } from 'rxjs';
import { SubscriptionDto } from '../../domains/subscriptions/models/subscription.model';

type ModalType = 'addSubscription' | 'editSubscription';

@Injectable({
  providedIn: 'root'
})
export class ModalService {

  openModal = signal<ModalType | null>(null);

  modalData = signal<{ modal: ModalType, data: any } | null>(null);

  triggerOpenModal(modalType: ModalType) {
    this.openModal.set(modalType);
  }

  closeModal() {
    this.openModal.set(null);
  }

  notifyModalData(modal: ModalType, data: any) {
    this.modalData.set({ modal, data });
    this.closeModal();
  }
}
