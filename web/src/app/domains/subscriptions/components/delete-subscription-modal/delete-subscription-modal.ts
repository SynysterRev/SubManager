import { Component, inject, input, output } from '@angular/core';
import { ModalService } from '../../../../core/services/modal';
import { SubscriptionService } from '../../services/subscription';
import { SubscriptionDto } from '../../models/subscription.model';

@Component({
  selector: 'app-delete-subscription-modal',
  imports: [],
  templateUrl: './delete-subscription-modal.html',
  styleUrl: './delete-subscription-modal.scss'
})
export class DeleteSubscriptionModal {
  subService = inject(SubscriptionService);
  modalService = inject(ModalService);

  subscription = input.required<SubscriptionDto>();
  confirm = output<void>();

  closeModal() {
    this.modalService.closeModal();
  }

  onConfirm() {
    this.confirm.emit();
  }
}
