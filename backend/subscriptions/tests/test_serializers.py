import datetime

import pytest
from rest_framework.exceptions import ValidationError

from subscriptions.models import Subscription
from subscriptions.serializers import (
    SubscriptionDetailSerializer,
    SubscriptionListSerializer,
)


@pytest.mark.django_db
def test_details_contains_expected_fields(base_subscription):
    data = SubscriptionDetailSerializer(instance=base_subscription).data
    expected_fields = {
        "id",
        "name",
        "category",
        "price",
        "payment_day",
        "is_active",
        "created_at",
        "user",
        "days_before_next_payment"
    }
    assert set(data.keys()) == expected_fields
    assert data["name"] == base_subscription.name
    assert data["category"] == base_subscription.category
    assert data["price"] == base_subscription.price
    assert data["is_active"] == base_subscription.is_active
    assert data["user"] == base_subscription.user.id
    assert data["payment_day"] == base_subscription.payment_day
    assert data[
               "days_before_next_payment"] == base_subscription.days_before_next_payment


@pytest.mark.django_db
def test_list_contains_expected_fields(base_subscription, base_user):
    sub2 = Subscription.objects.create(
        name="Test2",
        category="Test2",
        user=base_user,
        price=30.2,
        payment_day=5,
    )
    serializer = SubscriptionListSerializer(
        instance=[base_subscription, sub2], many=True
    )
    data = serializer.data

    assert isinstance(data, list)
    assert len(data) == 2
    expected_fields = {
        "id",
        "name",
        "category",
        "price",
        "is_active",
        "payment_day",
    }
    for i, sub in enumerate([base_subscription, sub2]):
        assert set(data[i].keys()) == expected_fields
        assert data[i]["id"] == sub.id
        assert data[i]["name"] == sub.name
        assert data[i]["category"] == sub.category
        assert data[i]["price"] == sub.price
        assert data[i]["is_active"] == sub.is_active
        assert data[i]["payment_day"] == sub.payment_day


@pytest.mark.django_db
@pytest.mark.parametrize(
    "field, type_value",
    [
        ("name", str),
        ("category", str),
        ("price", float),
        ("is_active", bool),
        ("payment_day", int),
        ("created_at", str),
        ("user", int),
        ("id", int),
    ],
)
def test_fields_correct_types(base_subscription, field, type_value):
    data = SubscriptionDetailSerializer(instance=base_subscription).data
    assert isinstance(data[field], type_value)


@pytest.mark.django_db
def test_payment_incorrect_value(base_user):
    data = {
        "name": "Test",
        "category": "Cat",
        "price": 10.0,
        "user": base_user.id,
        "payment_day": -5,
    }
    serializer = SubscriptionDetailSerializer(data=data)
    with pytest.raises(ValidationError):
        serializer.is_valid(raise_exception=True)


@pytest.mark.django_db
def test_list_ordered(base_subscription, base_user):
    sub2 = Subscription.objects.create(
        name="Test2",
        category="Test2",
        user=base_user,
        price=30.2,
        payment_day=5,
    )

    base_subscription.created_at = datetime.datetime.now() - datetime.timedelta(days=1)
    base_subscription.save(update_fields=["created_at"])

    subscriptions = Subscription.objects.all()
    serializer = SubscriptionListSerializer(instance=subscriptions, many=True)
    data = serializer.data
    assert data[0]["id"] == base_subscription.id
    assert data[1]["id"] == sub2.id


@pytest.mark.django_db
def test_empty_list():
    serializer = SubscriptionListSerializer(instance=[], many=True)
    data = serializer.data
    assert len(data) == 0
    assert data == []


@pytest.mark.django_db
def test_read_only_created_at_error(base_subscription):
    serializer = SubscriptionDetailSerializer(
        instance=base_subscription, data={"created_at": "2025-09-01"}, partial=True
    )
    assert serializer.is_valid()
    updated = serializer.save()
    assert updated.created_at == base_subscription.created_at


@pytest.mark.django_db
def test_read_only_user_error(base_subscription, user_model):
    new_user = user_model.objects.create_user(email="new@user.com", password="foo")
    serializer = SubscriptionDetailSerializer(
        instance=base_subscription, data={"user": new_user}, partial=True
    )
    assert serializer.is_valid()
    updated = serializer.save()
    assert updated.user == base_subscription.user
