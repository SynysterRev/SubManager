import pytest
from rest_framework.test import APIClient

from subscriptions.models import Subscription


@pytest.mark.django_db
def test_subscription_list_view(base_user, base_subscription):
    client = APIClient()
    client.force_authenticate(user=base_user)

    Subscription.objects.create(
        name="Test2",
        category="Test",
        price=30.0,
        payment_day=5,
        user=base_user,
    )

    response = client.get("/api/subscriptions/")
    assert response.status_code == 200

    data = response.json()

    expected_total = sum(s.price for s in Subscription.objects.filter(user=base_user))
    assert data["total_price"] == expected_total

    assert (
        len(data["subscriptions"])
        == Subscription.objects.filter(user=base_user).count()
    )


@pytest.mark.django_db
def test_subscription_list_empty_view(base_user):
    client = APIClient()
    client.force_authenticate(user=base_user)

    response = client.get("/api/subscriptions/")
    assert response.status_code == 200

    data = response.json()

    expected_total = 0
    assert data["total_price"] == expected_total

    assert (
        len(data["subscriptions"])
        == Subscription.objects.filter(user=base_user).count()
    )


@pytest.mark.django_db
def test_subscription_list_only_current_user_view(
    base_subscription, base_user, user_model
):
    client = APIClient()
    client.force_authenticate(user=base_user)

    new_user = user_model.objects.create_user(email="new@user.com", password="foo")
    Subscription.objects.create(
        name="Test2",
        category="Test",
        price=30.0,
        payment_day=4,
        user=new_user,
    )

    response = client.get("/api/subscriptions/")
    assert response.status_code == 200

    data = response.json()

    expected_total = sum(s.price for s in Subscription.objects.filter(user=base_user))
    assert data["total_price"] == expected_total

    assert (
        len(data["subscriptions"])
        == Subscription.objects.filter(user=base_user).count()
    )


@pytest.mark.django_db
def test_subscription_detail_view(base_user, base_subscription):
    client = APIClient()
    client.force_authenticate(user=base_user)

    url = f"/api/subscriptions/{base_subscription.id}/"
    response = client.get(url)
    assert response.status_code == 200

    data = response.json()

    assert data["id"] == base_subscription.id
    assert data["name"] == base_subscription.name


@pytest.mark.django_db
def test_create_subscription_view(base_user):
    client = APIClient()
    client.force_authenticate(user=base_user)

    data = {
        "name": "New",
        "category": "Cat",
        "price": 25.0,
        "payment_day": 5,
    }

    response = client.post("/api/subscriptions/", data, format="json")
    assert response.status_code == 201

    sub = Subscription.objects.get(name="New")
    assert sub.user == base_user


@pytest.mark.django_db
@pytest.mark.parametrize(
    "field",
    [
        "name",
        "category",
        "price",
        "payment_day",
    ],
)
def test_create_subscription_missing_field_view(base_user, field):
    client = APIClient()
    client.force_authenticate(user=base_user)

    data = {
        "name": "New",
        "category": "Cat",
        "price": 25.0,
        "payment_day": 5,
    }
    data.pop(field)

    response = client.post("/api/subscriptions/", data, format="json")
    assert response.status_code == 400
    response_data = response.json()
    assert field in response_data
    assert response_data[field] == ["This field is required."]


@pytest.mark.django_db
@pytest.mark.parametrize(
    "field, value",
    [
        ("name", None),
        ("category", None),
        ("price", None),
        ("payment_day", None),
    ],
)
def test_create_subscription_field_empty_view(base_user, field, value):
    client = APIClient()
    client.force_authenticate(user=base_user)

    data = {
        "name": "New",
        "category": "Cat",
        "price": 25.0,
        "payment_day": 5,
        field: value,
    }

    response = client.post("/api/subscriptions/", data, format="json")
    assert response.status_code == 400
    response_data = response.json()
    assert field in response_data
    assert response_data[field] == ["This field may not be null."]


@pytest.mark.django_db
@pytest.mark.parametrize(
    "field, value",
    [
        ("name", "Test"),
        ("category", "Test"),
        ("price", 10),
        ("payment_day", 5),
    ],
)
def test_put_subscription_view(base_subscription, base_user, field, value):
    client = APIClient()
    client.force_authenticate(user=base_user)

    data = {
        "name": base_subscription.name,
        "category": base_subscription.category,
        "price": base_subscription.price,
        "payment_day": base_subscription.payment_day,
        field: value,
    }

    response = client.put(
        f"/api/subscriptions/{base_subscription.id}/", data, format="json"
    )
    assert response.status_code == 200
    response_data = response.json()
    assert field in response_data
    assert response_data[field] == value


@pytest.mark.django_db
def test_put_subscription_forbidden_user_field_view(
    base_subscription, base_user, user_model
):
    client = APIClient()
    client.force_authenticate(user=base_user)
    new_user = user_model.objects.create_user(email="new@user.com", password="foo")
    data = {
        "name": base_subscription.name,
        "category": base_subscription.category,
        "price": base_subscription.price,
        "payment_day": base_subscription.payment_day,
        "user": new_user.id,
    }

    client.put(f"/api/subscriptions/{base_subscription.id}/", data, format="json")
    base_subscription.refresh_from_db()
    assert base_subscription.user == base_user


@pytest.mark.django_db
def test_delete_subscription(base_subscription, base_user):
    client = APIClient()
    client.force_authenticate(user=base_user)

    response = client.delete(f"/api/subscriptions/{base_subscription.id}/")

    assert response.status_code == 204
    assert response.content == b""

    with pytest.raises(Subscription.DoesNotExist):
        Subscription.objects.get(id=base_subscription.id)
