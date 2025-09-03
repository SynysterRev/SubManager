import pytest
from django.core.exceptions import ValidationError
from django.db import IntegrityError, transaction, connection

from subscriptions.models import Subscription


@pytest.mark.django_db
def test_create_subscription_success(user_model, base_user):
    sub = Subscription.objects.create(
        name="Test",
        category="Test",
        user=base_user,
        price=50.0,
        payment_day=5,
    )
    assert sub.name == "Test"
    assert sub.price == 50.0
    assert sub.user == base_user
    assert sub.category == "Test"
    assert sub.is_active is True


@pytest.mark.django_db
def test_create_subscription_with_nonexistent_user(user_model):
    with pytest.raises(IntegrityError):
        with transaction.atomic():
            Subscription.objects.create(
                name="Test",
                category="Test",
                user_id=999,  # ID does not exist
                price=15.99,
                payment_day=5,
            )
            connection.check_constraints()


@pytest.mark.django_db
@pytest.mark.parametrize(
    "field, value",
    [
        ("name", None),
        ("category", None),
        ("price", None),
        ("payment_day", None),
        ("user", None),
    ],
)
def test_subscription_required_fields(user_model, base_user, field, value):
    data = {
        "name": "Test",
        "category": "Flower",
        "price": 19.99,
        "payment_day": 5,
        "user": base_user,
    }

    data[field] = value

    sub = Subscription(**data)

    with pytest.raises(ValidationError):
        sub.full_clean()


@pytest.mark.django_db
def test_subscription_ordering(user_model, base_user):
    sub1 = Subscription.objects.create(
        name="A", category="Cat", price=10, user=base_user, payment_day=5
    )
    sub2 = Subscription.objects.create(
        name="B", category="Cat", price=20, user=base_user, payment_day=5
    )
    subs = list(Subscription.objects.all())
    assert subs == [sub1, sub2]
