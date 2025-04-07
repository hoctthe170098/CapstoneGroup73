import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LicdayRoutingModule } from './lichday-routing.module';
import { LichDayComponent } from './lichday.component';



@NgModule({
  declarations: [
    LichDayComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LicdayRoutingModule
  ]
})
export class LichdayModule { }
