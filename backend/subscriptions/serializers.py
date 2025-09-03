from rest_framework import serializers

from subscriptions.models import Subscription


class SubscriptionListSerializer(serializers.ModelSerializer):
    class Meta:
        model = Subscription
        fields = ["id", "name", "category", "price", "is_active", "payment_date"]


class SubscriptionDetailSerializer(serializers.ModelSerializer):
    user = serializers.PrimaryKeyRelatedField(read_only=True)
    created_at = serializers.DateTimeField(read_only=True)

    class Meta:
        model = Subscription
        fields = "__all__"
