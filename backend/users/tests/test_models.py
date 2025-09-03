import pytest


@pytest.mark.django_db
def test_create_user_success(user_model):
    """Test successful user creation"""
    user = user_model.objects.create_user(email="normal@user.com", password="foo")
    assert user.email == "normal@user.com"
    assert user.is_active
    assert not user.is_staff
    assert not user.is_superuser


@pytest.mark.django_db
def test_create_user_without_args_raises_error(user_model):
    """Test that creating user without arguments raises TypeError"""
    with pytest.raises(TypeError):
        user_model.objects.create_user()


@pytest.mark.django_db
def test_create_user_without_email_raises_error(user_model):
    """Test that creating user without email raises TypeError"""
    with pytest.raises(TypeError):
        user_model.objects.create_user(email="")


@pytest.mark.django_db
def test_create_user_empty_email_raises_error(user_model):
    """Test that creating user with empty email raises ValueError"""
    with pytest.raises(ValueError):
        user_model.objects.create_user(email="", password="foo")


@pytest.mark.django_db
def test_user_username_handling(user_model):
    """Test username field behavior (None for AbstractUser)"""
    user = user_model.objects.create_user(email="test@user.com", password="foo")
    try:
        assert user.username is None
    except AttributeError:
        # username doesn't exist for AbstractBaseUser
        pass


@pytest.mark.django_db
def test_create_superuser(user_model):
    admin_user = user_model.objects.create_superuser(
        email="super@user.com", password="foo"
    )
    assert admin_user.email == "super@user.com"
    assert admin_user.is_active
    assert admin_user.is_staff
    assert admin_user.is_superuser
    try:
        # username is None for the AbstractUser option
        # username does not exist for the AbstractBaseUser option
        assert admin_user.username is None
    except AttributeError:
        pass
    with pytest.raises(ValueError):
        user_model.objects.create_superuser(
            email="super@user.com", password="foo", is_superuser=False
        )
