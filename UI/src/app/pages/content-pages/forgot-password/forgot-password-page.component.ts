import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router, ActivatedRoute } from "@angular/router";

@Component({
    selector: 'app-forgot-password-page',
    templateUrl: './forgot-password-page.component.html',
    styleUrls: ['./forgot-password-page.component.scss']
})

export class ForgotPasswordPageComponent {
    @ViewChild('f') forgotPasswordForm!: NgForm;
    emailInvalid: boolean = false;

    constructor(private router: Router,
        private route: ActivatedRoute) { }
        validateEmail(email: string): boolean {
            const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
            return emailPattern.test(email);
        }

        onSubmit() {
            const emailValue = this.forgotPasswordForm.value.email;
    
            if (!emailValue || !this.validateEmail(emailValue)) {
                this.emailInvalid = true;
                return;
            }
    
            this.emailInvalid = false;
            this.forgotPasswordForm.reset();
        }

    // On login link click
    onLogin() {
        this.router.navigate(['login'], { relativeTo: this.route.parent });
    }

    // On registration link click
    onRegister() {
        this.router.navigate(['register'], { relativeTo: this.route.parent });
    }
}
