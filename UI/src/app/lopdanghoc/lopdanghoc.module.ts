import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LopdanghocRoutingModule } from './lopdanghoc-routing.module';
import { LopdanghocComponent } from './lopdanghoc.component';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    LopdanghocComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    LopdanghocRoutingModule
  ]
})
export class LopdanghocModule { }
