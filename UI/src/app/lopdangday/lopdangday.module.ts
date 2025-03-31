import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LopdangdayRoutingModule } from './lopdangday-routing.module';
import { LopdangdayComponent } from './lopdangday.component';
import { FormsModule } from '@angular/forms';


@NgModule({
  declarations: [
    LopdangdayComponent
  ],
  imports: [
    CommonModule,
    LopdangdayRoutingModule
  ]
})
export class LopdangdayModule { }
