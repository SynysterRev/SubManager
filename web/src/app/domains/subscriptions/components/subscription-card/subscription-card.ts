import { Component, computed, ElementRef, HostListener, input, output } from '@angular/core';
import { SubscriptionDto } from '../../models/subscription.model';
import { LocalDatePipe } from '../../pipes/local-date-pipe';

@Component({
  selector: 'app-subscription-card',
  imports: [LocalDatePipe],
  templateUrl: './subscription-card.html',
  styleUrl: './subscription-card.scss'
})
export class SubscriptionCard {

  isDropdownOpen: boolean = false;
  subscription = input.required<SubscriptionDto>();
  toggled = output<SubscriptionDto>();

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
}
