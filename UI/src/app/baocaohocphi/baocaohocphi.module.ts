import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { BaocaohocphiRoutingModule } from './baocaohocphi-routing.module';
import { BaocaohocphiComponent } from './baocaohocphi.component';


@NgModule({
  declarations: [
    BaocaohocphiComponent
  ],
  imports: [
    CommonModule,
    BaocaohocphiRoutingModule
  ]
})
export class BaocaohocphiModule { }
