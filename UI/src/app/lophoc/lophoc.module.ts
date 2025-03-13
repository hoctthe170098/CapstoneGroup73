import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LophocRoutingModule } from './lophoc-routing.module';
import { LophocComponent } from './lophoc.component';


@NgModule({
  declarations: [
    LophocComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    LophocRoutingModule
  ]
})
export class LophocModule { }
