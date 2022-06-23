import { AbstractControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
export function createPasswordStrengthValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;
    if (!value) {
      return null;
    }
    const hasUpperCase = /[A-Z]+/.test(value);
    const hasLowerCase = /[a-z]+/.test(value);
    const hasNumeric = /[0-9]+/.test(value);
    const passwordValid = hasUpperCase && hasLowerCase && hasNumeric;
    return !passwordValid ? { passwordStrength: true } : null;
  }
}

export function createConfirmPasswordValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const confirmPass = control.value;
    const pass = control.parent?.get('password')?.value;
    return pass === confirmPass ? null : { confirmPassword: true }
  }
}

export function createPhoneValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    let phone: string = control.value;
    phone = phone.trim();
    if (!phone || phone.length == 0) {
      return null;
    }
    const matchesRegex = /^[\+]?[0-9]{1,3}[0-9]{1,3}[0-9]{7}$/.test(phone);
    return matchesRegex ? null : { phone: true };
  }
}
