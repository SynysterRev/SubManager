from django.db.models import Sum
from rest_framework import viewsets
from rest_framework.response import Response

from subscriptions.models import Subscription
from subscriptions.serializers import (
    SubscriptionListSerializer,
    SubscriptionDetailSerializer,
)


class SubscriptionViewSet(viewsets.ModelViewSet):
    serializer_class = SubscriptionListSerializer

    def get_serializer_class(self):
        if self.action == "retrieve":
            return SubscriptionDetailSerializer
        return super().get_serializer_class()

    def get_queryset(self):
        user = self.request.user
        return Subscription.objects.filter(user=user)

    def perform_create(self, serializer):
        serializer.save(user=self.request.user)

    def list(self, request, *args, **kwargs):
        queryset = self.get_queryset()
        total_price = queryset.aggregate(total=Sum("price"))["total"] or 0
        serializer = self.get_serializer(queryset, many=True)
        return Response({"total_price": total_price, "subscriptions": serializer.data})
