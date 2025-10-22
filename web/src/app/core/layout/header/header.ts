import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ModalService } from '../../services/modal';

@Component({
  selector: 'app-header',
  imports: [RouterModule],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header {
  modalService = inject(ModalService);

  onAddSubcriptionClick() {
    this.modalService.triggerOpenModal('addSubscription');
  }
}
