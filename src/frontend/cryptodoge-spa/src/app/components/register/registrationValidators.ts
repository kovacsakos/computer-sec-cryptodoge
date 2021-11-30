import { AbstractControl, ValidationErrors } from "@angular/forms";

class Helpers {
    static IsUpper(c: string) {
        return c >= 'A' && c <= 'Z';
    }

    static IsLower(c: string) {
        return c >= 'a' && c <= 'z';
    }

    static IsDigit(c: string) {
        return c >= '0' && c <= '9';
    }

    static IsLetterOrDigit(c: string) {
        return this.IsUpper(c) || this.IsLower(c) || this.IsDigit(c);
    }

    static StringOnlyLetterOrDigit(s: string) {
        for (let i = 0; i<s.length; i++) {
            if (!this.IsLetterOrDigit(s.charAt(i))) {
                return false;
            }
        }
        return true;
    }

    static StringHasAlphaNumeric(s: string) {
        if (s == null) {return false;}
        return !this.StringOnlyLetterOrDigit(s);
    }
}

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
        let passwordString = control.value as string;
        return Helpers.StringHasAlphaNumeric(passwordString) ?  null : ({nonAlphaNumeric: true});

    }

    static passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
        return (control.get('password').value === control.get('confirmPassword').value) ? null : ({mismatch: true});
    }
}
    
