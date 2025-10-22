import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubscriptionTotalCard } from './subscription-total-card';

describe('SubscriptionTotalCard', () => {
  let component: SubscriptionTotalCard;
  let fixture: ComponentFixture<SubscriptionTotalCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SubscriptionTotalCard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SubscriptionTotalCard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
