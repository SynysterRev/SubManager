import calendar
from datetime import date

from dateutil.relativedelta import relativedelta
from django.core.validators import MinValueValidator, MaxValueValidator
from django.db import models
from django.utils import timezone

from subApi import settings
from utils.math import clamp


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

    def date_next_payment(self):
        today_date = timezone.now().date()
        first_day, num_days = calendar.monthrange(today_date.year, today_date.month)
        payment_day = clamp(self.payment_day, 1, num_days)

        this_month_payment = date(today_date.year, today_date.month, payment_day)
        next_month_payment = this_month_payment + relativedelta(months=1)
        if today_date.day <= payment_day:
            next_payment = this_month_payment
        else:
            next_payment = next_month_payment

        return next_payment

    @property
    def days_before_next_payment(self):
        today_date = timezone.now().date()
        next_payment = self.date_next_payment()
        return (next_payment - today_date).days
