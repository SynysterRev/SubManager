from django.core.validators import MinValueValidator, MaxValueValidator
from django.db import models

from subApi import settings


class Subscription(models.Model):
    name = models.CharField(max_length=100)
    category = models.CharField(max_length=100)
    price = models.FloatField()
    payment_day = models.IntegerField(
        validators=[MinValueValidator(1), MaxValueValidator(31)]
    )
    is_active = models.BooleanField(default=True)
    user = models.ForeignKey(
        settings.AUTH_USER_MODEL, on_delete=models.CASCADE, related_name="subscriptions"
    )
    created_at = models.DateTimeField(auto_now_add=True)

    class Meta:
        ordering = ["created_at"]
