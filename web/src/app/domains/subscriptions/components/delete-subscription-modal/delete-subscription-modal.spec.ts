import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteSubscriptionModal } from './delete-subscription-modal';

describe('DeleteSubscriptionModal', () => {
  let component: DeleteSubscriptionModal;
  let fixture: ComponentFixture<DeleteSubscriptionModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeleteSubscriptionModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DeleteSubscriptionModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
