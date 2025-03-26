import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms'; // 🆕 thêm ReactiveFormsModule

import { ChinhsachRoutingModule } from './chinhsach-routing.module';
import { ChinhsachComponent } from './chinhsach.component';

@NgModule({
  declarations: [
    ChinhsachComponent
  ],
  imports: [
    CommonModule,
    ChinhsachRoutingModule,
    FormsModule,
    ReactiveFormsModule 
  ],
    exports: [
     ChinhsachComponent
    ]
})
export class ChinhsachModule { }
