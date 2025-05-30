import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LophocRoutingModule } from './lophoc-routing.module';
import { LophocComponent } from './lophoc.component';
import { AddlophocComponent } from './addlophoc/addlophoc.component';
import { EditLopHocComponent } from './editlophoc/editlophoc.component';
import { BaocaodiemdanhquanlycosoComponent } from './baocaodiemdanhquanlycoso/baocaodiemdanhquanlycoso.component';
import { BaocaohocphiComponent } from './baocaohocphi/baocaohocphi.component';
import { NgSelectModule } from '@ng-select/ng-select'

@NgModule({
  declarations: [
    LophocComponent,
    AddlophocComponent,
    EditLopHocComponent,
    BaocaodiemdanhquanlycosoComponent,
    BaocaohocphiComponent
  ],
  imports: [
    NgSelectModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LophocRoutingModule
  ]
})
export class LophocModule { }
