import { Component, ElementRef, HostListener, input } from '@angular/core';
import { SubscriptionDto } from '../../models/subscription.model';

@Component({
  selector: 'app-subscription-card',
  imports: [],
  templateUrl: './subscription-card.html',
  styleUrl: './subscription-card.scss'
})
export class SubscriptionCard {

  isDropdownOpen: boolean = false;
  subscription = input.required<SubscriptionDto>();

  constructor(private elementRef: ElementRef) { }

  toggleDropdown(event: Event) {
    event.stopPropagation();
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  @HostListener('document:click', ['$event'])
  onClickOutside(event: Event) {
    if (this.isDropdownOpen) {
      if (!this.elementRef.nativeElement.contains(event.target)) {
        this.isDropdownOpen = false;
      }
    }
  }
}
