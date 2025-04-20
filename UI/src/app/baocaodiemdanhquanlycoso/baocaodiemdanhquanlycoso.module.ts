import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BaocaodiemdanhquanlycosoRoutingModule } from './baocaodiemdanhquanlycoso-routing.module';
import { BaocaodiemdanhquanlycosoComponent } from './baocaodiemdanhquanlycoso.component';


@NgModule({
  declarations: [
    BaocaodiemdanhquanlycosoComponent
  ],
  imports: [
    FormsModule,
    CommonModule,
    BaocaodiemdanhquanlycosoRoutingModule
  ]
})
export class BaocaodiemdanhquanlycosoModule { }
