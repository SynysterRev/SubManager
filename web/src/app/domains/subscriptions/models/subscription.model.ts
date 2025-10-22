export interface BaseSubscription {
    name: string;
    category: string;
    price: number;
}

export interface SubscriptionDto extends BaseSubscription {
    id: number;
    createdAt: Date;
    daysBeforeNextPayment: number;
    paymentDate: Date;
    yearCost: number;
    isActive: boolean;
}

export interface SubscriptionCreateDto extends BaseSubscription {
    paymentDay: number;
}

export interface SubscriptionUpdateDto {
    name?: string;
    category?: string;
    price?: number;
    isActive?: boolean;
    paymentDay?: number;
}
