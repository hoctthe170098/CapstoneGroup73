import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CosoRoutingModule } from './coso-routing.module';
import { CosoComponent } from './coso.component';

@NgModule({
    declarations: [
        CosoComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        CosoRoutingModule
    ],
    exports: [
        CosoComponent
    ]
})
export class CosoModule { }
