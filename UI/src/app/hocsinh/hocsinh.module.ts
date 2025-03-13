import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {HocSinhRoutingModule  } from './hocsinh-routing.module';
import { HocsinhComponent } from './hocsinh.component';
import { ImportHocsinhComponent } from './import-hocsinh/import-hocsinh/import-hocsinh.component';

@NgModule({
    declarations: [
        HocsinhComponent,
        ImportHocsinhComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        HocSinhRoutingModule
    ],
    exports: [
        HocsinhComponent
    ]
})
export class HocSinhModule { }
