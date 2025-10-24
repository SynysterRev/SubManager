import { Component, computed, ElementRef, HostListener, inject, input, output } from '@angular/core';
import { SubscriptionDto } from '../../models/subscription.model';
import { LocalDatePipe } from '../../pipes/local-date-pipe';
import { Dropdown } from "../../../../shared/components/dropdown/dropdown";
import { SubCardDropdown } from '../../../../shared/types/dropdown.type';
import { ModalService } from '../../../../core/services/modal';

@Component({
  selector: 'app-subscription-card',
  imports: [LocalDatePipe, Dropdown],
  templateUrl: './subscription-card.html',
  styleUrl: './subscription-card.scss'
})
export class SubscriptionCard {

  isDropdownOpen: boolean = false;
  subscription = input.required<SubscriptionDto>();
  toggled = output<SubscriptionDto>();
  modalService = inject(ModalService);

  constructor(private elementRef: ElementRef) { }

  toggleActive() {
    const sub = this.subscription();
    this.toggled.emit({ ...sub, isActive: !sub.isActive });
  }

  toggleDropdown(event: Event) {
    event.stopPropagation();
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  cardClass = computed(() =>
    this.subscription().isActive ? 'card-active' : 'card-not-active'
  );

  @HostListener('document:click', ['$event'])
  onClickOutside(event: Event) {
    if (this.isDropdownOpen) {
      if (!this.elementRef.nativeElement.contains(event.target)) {
        this.isDropdownOpen = false;
      }
    }
  }

  onDropdownItemClick(event: string) {
    const selectedItem = event as SubCardDropdown;
    switch (selectedItem) {
      case 'Edit':
        this.modalService.triggerOpenModal('editSubscription', this.subscription());
        break;
      case 'Delete':
        console.log("delete");
        this.modalService.triggerOpenModal('deleteSubscription', this.subscription());
        break;
    }
  }
}
