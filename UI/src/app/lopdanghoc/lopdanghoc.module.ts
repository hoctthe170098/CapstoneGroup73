import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LopdanghocRoutingModule } from './lopdanghoc-routing.module';
import { LopdanghocComponent } from './lopdanghoc.component';


@NgModule({
  declarations: [
    LopdanghocComponent
  ],
  imports: [
    CommonModule,
    LopdanghocRoutingModule
  ]
})
export class LopdanghocModule { }
