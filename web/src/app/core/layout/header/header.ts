import { Component, inject, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ModalService } from '../../services/modal';
import { Dropdown } from "../../../shared/components/dropdown/dropdown";
import { AuthService } from '../../../domains/auth/services/auth';
import { AccountDropdown } from '../../../shared/types/dropdown.type';

@Component({
  selector: 'app-header',
  imports: [RouterModule, Dropdown,],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header {
  @ViewChild('accountDropdown') accountDropdown!: Dropdown<AccountDropdown>;
  modalService = inject(ModalService);
  authService = inject(AuthService);

  onAddSubcriptionClick() {
    this.modalService.triggerOpenModal('addSubscription');
  }

  onDropdownItemClick(event: string) {
    const selectedItem = event as AccountDropdown;
    switch (selectedItem) {
      case 'Profile':
        console.log("profile");
        break;
      case 'Settings':
        console.log("Settings");
        break;
      case 'Logout':
        this.authService.logout();
        break;
    }
  }
}
