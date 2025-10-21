import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function passwordMatchValidator(): ValidatorFn {
    return (formGroup: AbstractControl): ValidationErrors | null => {
        const password = formGroup.get('password')?.value;
        const confirmPassword = formGroup.get('confirmPassword')?.value;

        // Si un des deux est vide, on ne valide pas (géré par Validators.required)
        if (!password || !confirmPassword) {
            return null;
        }

        // Si différents, retourne une erreur
        if (password !== confirmPassword) {
            return { passwordMismatch: true };
        }

        // Si identiques, pas d'erreur
        return null;
    };
}