import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { BaocaodiemdanhquanlycosoRoutingModule } from './baocaodiemdanhquanlycoso-routing.module';
import { BaocaodiemdanhquanlycosoComponent } from './baocaodiemdanhquanlycoso.component';


@NgModule({
  declarations: [
    BaocaodiemdanhquanlycosoComponent
  ],
  imports: [
    CommonModule,
    BaocaodiemdanhquanlycosoRoutingModule
  ]
})
export class BaocaodiemdanhquanlycosoModule { }
