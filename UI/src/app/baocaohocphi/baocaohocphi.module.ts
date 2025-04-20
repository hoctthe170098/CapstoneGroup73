import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BaocaohocphiRoutingModule } from './baocaohocphi-routing.module';
import { BaocaohocphiComponent } from './baocaohocphi.component';


@NgModule({
  declarations: [
    BaocaohocphiComponent
  ],
  imports: [
    CommonModule,
    BaocaohocphiRoutingModule,
    FormsModule
  ]
})
export class BaocaohocphiModule { }
