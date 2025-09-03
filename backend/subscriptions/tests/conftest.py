import pytest

from subscriptions.models import Subscription


@pytest.fixture
def user_model():
    from django.contrib.auth import get_user_model

    return get_user_model()


@pytest.fixture
def base_user(user_model):
    return user_model.objects.create_user(email="normal@user.com", password="foo")


@pytest.fixture
def base_subscription(base_user):
    return Subscription.objects.create(
        name="Test",
        category="Test",
        user=base_user,
        price=50.0,
        payment_day=5,
    )
