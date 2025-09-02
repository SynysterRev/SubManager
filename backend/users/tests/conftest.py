import pytest


@pytest.fixture
def user_model():
    from django.contrib.auth import get_user_model

    return get_user_model()
