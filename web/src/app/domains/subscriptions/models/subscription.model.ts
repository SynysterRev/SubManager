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
    paymentDay: number;
    isActive: boolean;
}

export interface SubscriptionCreateDto extends BaseSubscription {
    paymentDay: number;
}

export interface SubscriptionUpdateDto extends Partial<BaseSubscription> {
    isActive?: boolean;
    paymentDay?: number;
}

export interface SubscriptionFormData extends Partial<BaseSubscription> {
    isActive?: boolean;
    paymentDay?: number;
}
