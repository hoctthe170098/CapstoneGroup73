import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LophocRoutingModule } from './lophoc-routing.module';
import { LophocComponent } from './lophoc.component';
import { AddlophocComponent } from './addlophoc/addlophoc.component';
import { EditLopHocComponent } from './editlophoc/editlophoc.component';


@NgModule({
  declarations: [
    LophocComponent,
    AddlophocComponent,
    EditLopHocComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LophocRoutingModule
  ]
})
export class LophocModule { }
