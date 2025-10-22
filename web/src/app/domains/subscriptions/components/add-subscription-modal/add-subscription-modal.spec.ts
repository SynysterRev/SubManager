import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddSubscriptionModal } from './add-subscription-modal';

describe('AddSubscriptionModal', () => {
  let component: AddSubscriptionModal;
  let fixture: ComponentFixture<AddSubscriptionModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddSubscriptionModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddSubscriptionModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
