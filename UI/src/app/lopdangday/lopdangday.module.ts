import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LopdangdayRoutingModule } from './lopdangday-routing.module';
import { LopdangdayComponent } from './lopdangday.component';
import { DiemdanhComponent } from './diemdanh/diemdanh.component';
import { DanhsachhocsinhComponent } from './danhsachhocsinh/danhsachhocsinh.component';
import { BaitapComponent } from './baitap/baitap.component';
import { LichkiemtraComponent } from './lichkiemtra/lichkiemtra.component';
import { ChiTietComponent } from './chi-tiet/chi-tiet.component';


@NgModule({
  declarations: [
    LopdangdayComponent,
    DiemdanhComponent,
    DanhsachhocsinhComponent,
    BaitapComponent,
    LichkiemtraComponent,
    ChiTietComponent
  ],
  imports: [
    CommonModule,
    LopdangdayRoutingModule
  ]
})
export class LopdangdayModule { }
