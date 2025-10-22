import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubscriptionsDashboard } from './subscriptions-dashboard';

describe('SubscriptionsDashboard', () => {
  let component: SubscriptionsDashboard;
  let fixture: ComponentFixture<SubscriptionsDashboard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SubscriptionsDashboard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SubscriptionsDashboard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
