import { AbstractControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { BookService } from '../services/book.service';
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

export function createIsbnValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    let isbn: string = control.value;
    return BookService.isValidIsbn(isbn) ? null : { isbn: true };
  }
}

export function createYearValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const year: number = control.value;
    const borderYear = (new Date().getFullYear() + 2);
    return year >= 0 && year <= borderYear ? null : { year: true };
  }
}

export function createNumValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const num: number = control.value;
    return num >= 0 ? null : { num: true };
  }
}
