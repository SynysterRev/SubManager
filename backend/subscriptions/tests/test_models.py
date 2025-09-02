from datetime import date

import pytest
from django.core.exceptions import ValidationError
from django.db import IntegrityError, transaction, connection

from subscriptions.models import Subscription


@pytest.mark.django_db
def test_create_subscription_success(user_model):
    user = user_model.objects.create_user(email="normal@user.com", password="foo")
    sub = Subscription.objects.create(name="Test", category="Test", user=user,
                                      price=50.0, payment_date=date.today())
    assert sub.name == "Test"
    assert sub.price == 50.0
    assert sub.user == user
    assert sub.category == "Test"
    assert sub.is_active == True


@pytest.mark.django_db
def test_create_subscription_with_nonexistent_user(user_model):
    with pytest.raises(IntegrityError):
        with transaction.atomic():
            Subscription.objects.create(
                name="Test",
                category="Test",
                user_id=999,  # ID does not exist
                price=15.99,
                payment_date=date.today()
            )
            connection.check_constraints()


@pytest.mark.django_db
@pytest.mark.parametrize("field, value", [
    ("name", None),
    ("category", None),
    ("price", None),
    ("payment_date", None),
    ("user", None),
])
def test_subscription_required_fields(user_model, field, value):
    user = user_model.objects.create_user(email="normal@user.com", password="foo")

    data = {
        "name": "Test",
        "category": "Flower",
        "price": 19.99,
        "payment_date": date.today(),
        "user": user,
    }

    data[field] = value

    sub = Subscription(**data)

    with pytest.raises(ValidationError):
        sub.full_clean()


@pytest.mark.django_db
def test_subscription_ordering(user_model):
    user = user_model.objects.create_user(email="normal@user.com", password="foo")
    sub1 = Subscription.objects.create(
        name="A", category="Cat", price=10, user=user, payment_date=date.today()
    )
    sub2 = Subscription.objects.create(
        name="B", category="Cat", price=20, user=user, payment_date=date.today()
    )
    subs = list(Subscription.objects.all())
    assert subs == [sub1, sub2]
