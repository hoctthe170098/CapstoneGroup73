import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SlotComponent } from './slot.component';
import { SlotRoutingModule } from './slot-routing.module';

@NgModule({
  declarations: [
    SlotComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SlotRoutingModule
  ],
  exports: [
    SlotComponent
  ]
})
export class SlotModule { }
