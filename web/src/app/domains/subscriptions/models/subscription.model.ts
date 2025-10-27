export interface BaseSubscription {
    name: string;
    categoryId?: number;
    price: number;
    currencyCode: string;
}

export interface SubscriptionDto extends BaseSubscription {
    id: number;
    createdAt: Date;
    daysBeforeNextPayment: number;
    paymentDate: Date;
    yearCost: number;
    paymentDay: number;
    categoryName?: string;
    isActive: boolean;
}

export interface SubscriptionCreateDto extends BaseSubscription {
    paymentDay: number;
}

export interface SubscriptionUpdateDto extends Partial<BaseSubscription> {
    isActive?: boolean;
    categoryName?: string;
    paymentDay?: number;
}

export interface SubscriptionFormData extends Partial<BaseSubscription> {
    isActive?: boolean;
    paymentDay?: number;
}
