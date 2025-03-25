import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ChinhsachRoutingModule } from './chinhsach-routing.module';
import { ChinhsachComponent } from './chinhsach.component';


@NgModule({
  declarations: [
    ChinhsachComponent
  ],
  imports: [
    CommonModule,
    ChinhsachRoutingModule
  ]
})
export class ChinhsachModule { }
