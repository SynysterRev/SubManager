export interface LoginDto {
    email: string;
    password: string;
}

export interface RegisterDto {
    email: string;
    password: string;
    confirmPassword: string;
}

export interface TokenDto {
    email: string;
    token: string;
    expiration: string;
    currencyCode: string;
}