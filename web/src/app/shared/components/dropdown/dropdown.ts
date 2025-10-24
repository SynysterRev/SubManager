import { Component, ElementRef, HostListener, input, output } from '@angular/core';

@Component({
  selector: 'app-dropdown',
  imports: [],
  templateUrl: './dropdown.html',
  styleUrl: './dropdown.scss'
})
export class Dropdown<T = string> {
  isOpen = false;
  itemClick = output<T>();


  constructor(private elementRef: ElementRef) { }

  toggleDropdown(event: Event) {
    event.stopPropagation();
    this.isOpen = !this.isOpen;
  }

  onMenuItemClick(event: Event, itemData: T) {
    event.stopPropagation();
    this.isOpen = false;
    this.itemClick.emit(itemData);
  }

  @HostListener('document:click', ['$event'])
  onClickOutside(event: Event) {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.isOpen = false;
    }
  }
}