import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LichhocRoutingModule } from './lichhoc-routing.module';
import { LichhocComponent } from './lichhoc.component';


@NgModule({
  declarations: [
    LichhocComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LichhocRoutingModule
  ]
})
export class LichhocModule { }
