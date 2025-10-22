import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActiveSubscriptionsCard } from './active-subscriptions-card';

describe('ActiveSubscriptionsCard', () => {
  let component: ActiveSubscriptionsCard;
  let fixture: ComponentFixture<ActiveSubscriptionsCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ActiveSubscriptionsCard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ActiveSubscriptionsCard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
