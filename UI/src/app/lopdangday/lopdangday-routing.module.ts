import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LopdangdayComponent } from './lopdangday.component';
import { BaitapComponent } from './baitap/baitap.component';
import { DanhsachhocsinhComponent } from './danhsachhocsinh/danhsachhocsinh.component';
import { DiemdanhComponent } from './diemdanh/diemdanh.component';
import { LichkiemtraComponent } from './lichkiemtra/lichkiemtra.component';
import { ChiTietComponent } from './chi-tiet/chi-tiet.component';
import { ChitietBaitapComponent } from './baitap/chitiet-baitap/chitiet-baitap.component';
import { BaocaodiemdanhComponent } from './baocaodiemdanh/baocaodiemdanh.component';
import { BaocaodiemComponent } from './baocaodiem/baocaodiem.component';
const routes: Routes = [
  {
    path: '',
    component: LopdangdayComponent,
    data: {
      title: "lopdangday",
    },
  },

  { path: "baitap", component: BaitapComponent },
  { path: "danhsachhocsinh", component: DanhsachhocsinhComponent },
  { path: "diemdanh", component: DiemdanhComponent },
  { path: "lichkiemtra", component: LichkiemtraComponent },
  
  
  { path: "baocaodiemdanh", component: BaocaodiemdanhComponent },
  { path: "baocaodiem", component: BaocaodiemComponent },

  {
    path: "chi-tiet/:tenLop",
    component: ChiTietComponent,
    children: [
      { path: "", redirectTo: "danhsachhocsinh", pathMatch: "full" },
      { path: "baitap", component: BaitapComponent },
      { path: "baitap/:baiTapId", component: ChitietBaitapComponent },
      { path: "danhsachhocsinh", component: DanhsachhocsinhComponent },
      { path: "diemdanh", component: DiemdanhComponent },
      { path: "lichkiemtra", component: LichkiemtraComponent },
      { path: "baocaodiemdanh", component: BaocaodiemdanhComponent },
      { path: "baocaodiem", component: BaocaodiemComponent },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class LopdangdayRoutingModule {}
