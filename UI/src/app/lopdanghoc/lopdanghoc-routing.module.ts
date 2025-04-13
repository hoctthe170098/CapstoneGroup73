import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LopdanghocComponent } from './lopdanghoc.component';
import { ChiTietComponent } from './chi-tiet/chi-tiet.component';
import { BaiTapComponent } from './bai-tap/bai-tap.component';
import { LichThiComponent } from './lich-thi/lich-thi.component';
import { BaocaoDiemComponent } from './baocao-diem/baocao-diem.component';
import { BaocaoDiemdanhComponent } from './baocao-diemdanh/baocao-diemdanh.component';
import { ChitietBaitaphocsinhComponent } from './bai-tap/chitiet-baitaphocsinh/chitiet-baitaphocsinh.component';

const routes: Routes = [
  {
    path: '',
    component: LopdanghocComponent,
    data: {
      title: 'Lớp đang học'
    }
  },
  {
    path: 'chi-tiet/:tenLop',
    component: ChiTietComponent,
    children: [
      { path: 'bai-tap', component: BaiTapComponent },
      { path: 'bai-tap/:baiTapId', component: ChitietBaitaphocsinhComponent }
    ]
  },
  {
    path: 'chi-tiet/:tenLop',
    component: ChiTietComponent,
    children: [
      { path: '', redirectTo: 'bai-tap', pathMatch: 'full' },
      { path: 'bai-tap', component: BaiTapComponent },
      { path: 'lich-thi', component: LichThiComponent },
      { path: 'baocao-diem', component: BaocaoDiemComponent },
      { path: 'baocao-diemdanh', component: BaocaoDiemdanhComponent },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})

export class LopdanghocRoutingModule { }
