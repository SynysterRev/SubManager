export interface BaseSubscription {
    name: string;
    category: string;
    price: number;
}

export interface Subscription extends BaseSubscription {
    id: number;
    createdAt: Date;
    daysBeforeNextPayment: number;
    paymentDate: Date;
    yearCost: number;
    isActive: boolean;
}

export interface SubscriptionCreate extends BaseSubscription {
    paymentDay: number;
}

export interface SubscriptionUpdate {
    name?: string;
    category?: string;
    price?: number;
    isActive?: boolean;
    paymentDay?: number;
}
