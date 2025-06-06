import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LopdangdayRoutingModule } from './lopdangday-routing.module';
import { LopdangdayComponent } from './lopdangday.component';
import { DiemdanhComponent } from './diemdanh/diemdanh.component';
import { DanhsachhocsinhComponent } from './danhsachhocsinh/danhsachhocsinh.component';
import { BaitapComponent } from './baitap/baitap.component';
import { LichkiemtraComponent } from './lichkiemtra/lichkiemtra.component';
import { ChiTietComponent } from './chi-tiet/chi-tiet.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ChitietBaitapComponent } from './baitap/chitiet-baitap/chitiet-baitap.component';
import { BaocaodiemdanhComponent } from './baocaodiemdanh/baocaodiemdanh.component';
import { BaocaodiemComponent } from './baocaodiem/baocaodiem.component';
import { DiemkiemtraComponent } from './diemkiemtra/diemkiemtra.component';
import { NhanxetdinhkiComponent } from './nhanxetdinhki/nhanxetdinhki.component';
import { ChuongtrinhhocComponent } from './chuongtrinhhoc/chuongtrinhhoc.component';

@NgModule({
  declarations: [
    LopdangdayComponent,
    DiemdanhComponent,
    DanhsachhocsinhComponent,
    BaitapComponent,
    LichkiemtraComponent,
    ChiTietComponent,
    ChitietBaitapComponent,
    BaocaodiemdanhComponent,
    BaocaodiemComponent,
    DiemkiemtraComponent,
    NhanxetdinhkiComponent,
    ChuongtrinhhocComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LopdangdayRoutingModule
  ]
})
export class LopdangdayModule { }
