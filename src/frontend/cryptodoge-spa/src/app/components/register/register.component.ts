import { RegisterDto } from './../../../../generated/client/model/registerDto';
import { Component, OnInit } from '@angular/core';
import { AuthService } from 'generated/client';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { UserService } from 'src/app/services/user.service';
import { UserMainPageService } from 'src/app/services/user-main-page.service';
import { Router } from '@angular/router';
import { ToastService } from 'src/app/services/toast.service';
import { RegistrationValidators } from './RegistrationValidators';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  newUser: RegisterDto = {}
  registerForm: FormGroup;
  constructor(private authService: AuthService, 
              private userService: UserService, 
              private fb: FormBuilder, 
              private router: Router,
              private toast: ToastService,
              private mainPageService: UserMainPageService) { }

  ngOnInit() {
    this.createReisterForm();
  }

  createReisterForm() {
    this.registerForm = this.fb.group({
      
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8), RegistrationValidators.passwordHasLowerLetter, RegistrationValidators.passwordHasUpperLetter, RegistrationValidators.passwordHasDigits, RegistrationValidators.passwordHasNonAlphaNumeric]],
      confirmPassword: ['', Validators.required]
    }, {validators: RegistrationValidators.passwordMatchValidator});
  }

  register() {
    if (this.registerForm.valid) {
      this.newUser.emailAddress = this.registerForm.value.email;
      this.newUser.password = this.registerForm.value.password;
      this.newUser.userName = this.registerForm.value.username;

      this.authService.authRegister(this.newUser).subscribe(tokens => {
        this.userService.setTokens(tokens);
        this.toast.success('Registration successful');
      }, error => {
        this.toast.error('Error during registration');
      }, () => {
        if (this.userService.hasRole(['ADMIN'])) {
          this.router.navigate(this.mainPageService.getMainPageForUser());
        }
        else {
          this.router.navigate(this.mainPageService.getMainPageForUser());
        }
      })
    }
  }

  userNameHasErrors() {
    return this.registerForm.get('username').errors && this.registerForm.get('username').touched;
  }

  emailHasErrors() {
    return (this.registerForm.get('email').errors || this.registerForm.get('email').hasError('email')) && this.registerForm.get('email').touched;
  }

  passwordHasErrors(error: 'required' | 'noLower' | 'noUpper' |'minlength' | 'empty' | 'noDigit' | 'nonAlphaNumeric') {
    if (error == 'empty') {
      return this.registerForm.get('password').errors && this.registerForm.get('password').touched;
    }
    return this.registerForm.get('password').hasError(error) && this.registerForm.get('password').touched;
  }

  getPasswordErrorMessage() {
    if (this.passwordHasErrors('required'))
      return "Password is required";
    if (this.passwordHasErrors('minlength'))
      return "The password should be at least 8 characters long";
    if (this.passwordHasErrors('noLower'))
      return "The password should contain at least one lowercase letter";
    if (this.passwordHasErrors('noUpper'))
      return "The password should contain at least one uppercase letter";
    if (this.passwordHasErrors('noDigit'))
      return "The password should contain at least one digit";
    if (this.passwordHasErrors('nonAlphaNumeric'))
      return "The password should contain at least one non alphanumeric character";
    return '-'; // We can't return empty string because it won't take up the space on the UI
  }

  confirmPasswordHasErrors(component: 'input' | 'help') {
    if (component == 'help')
      return this.registerForm.hasError('mismatch') && this.registerForm.get('confirmPassword').touched;
    if (component == 'input')
      return this.registerForm.get('confirmPassword').errors && this.registerForm.get('confirmPassword').touched || this.registerForm.get('confirmPassword').touched && this.registerForm.hasError('mismatch');
  }
}
