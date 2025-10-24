import { Injectable, signal } from '@angular/core';

type ModalType = 'addSubscription' | 'editSubscription' | 'deleteSubscription';

@Injectable({
  providedIn: 'root'
})
export class ModalService {

  openModal = signal<ModalType | null>(null);

  modalData = signal<{ modal: ModalType, data: any } | null>(null);

  triggerOpenModal(modalType: ModalType, data?: any) {
    if (data) {
      this.modalData.set({ modal: modalType, data });
    }
    this.openModal.set(modalType);
  }

  closeModal() {
    this.openModal.set(null);
    this.modalData.set(null);
  }

  notifyModalData(modal: ModalType, data: any) {
    this.modalData.set({ modal, data });
    this.closeModal();
  }
}
