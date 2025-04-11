import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LopdanghocRoutingModule } from './lopdanghoc-routing.module';
import { LopdanghocComponent } from './lopdanghoc.component';
import { FormsModule } from '@angular/forms';
import { ChiTietComponent } from './chi-tiet/chi-tiet.component';
import { BaiTapComponent } from './bai-tap/bai-tap.component';
import { LichThiComponent } from './lich-thi/lich-thi.component';
import { BaocaoDiemComponent } from './baocao-diem/baocao-diem.component';
import { BaocaoDiemdanhComponent } from './baocao-diemdanh/baocao-diemdanh.component';

@NgModule({
  declarations: [
    LopdanghocComponent,
    ChiTietComponent,
    BaiTapComponent,
    LichThiComponent,
    BaocaoDiemComponent,
    BaocaoDiemdanhComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    LopdanghocRoutingModule
  ]
})
export class LopdanghocModule { }
