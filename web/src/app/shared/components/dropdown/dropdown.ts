import { Component, ElementRef, HostListener, input, output } from '@angular/core';

@Component({
  selector: 'app-dropdown',
  imports: [],
  templateUrl: './dropdown.html',
  styleUrl: './dropdown.scss'
})
export class Dropdown {
  isOpen = false;
  itemClick = output<string>();


  constructor(private elementRef: ElementRef) { }

  toggleDropdown(event: Event) {
    event.stopPropagation();
    this.isOpen = !this.isOpen;
  }

  onMenuItemClick(event: Event, itemData: any = null) {
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