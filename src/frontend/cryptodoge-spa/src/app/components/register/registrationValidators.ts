import { AbstractControl, ValidationErrors } from "@angular/forms";

export class RegistrationValidators {
    static passwordHasUpperLetter(control: AbstractControl): ValidationErrors | null {
        return /[A-Z]/.test(control.value) ? null : ({noUpper: true});
    }

    static passwordHasLowerLetter(control: AbstractControl): ValidationErrors | null {
        return /[a-z]/.test(control.value) ?  null : ({noLower: true});
    }

    static passwordHasDigits(control: AbstractControl): ValidationErrors | null {
        return /\d/.test(control.value) ?  null : ({noDigit: true});
    }

    static passwordHasNonAlphaNumeric(control: AbstractControl): ValidationErrors | null {
        return /([^a-zA-Z\d])+([a-zA-Z\d])+|([a-zA-Z\d])+([^a-zA-Z\d])+/.test(control.value) ?  null : ({nonAlphaNumeric: true});
    }

    static passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
        return (control.get('password').value === control.get('confirmPassword').value) ? null : ({mismatch: true});
    }    
}
