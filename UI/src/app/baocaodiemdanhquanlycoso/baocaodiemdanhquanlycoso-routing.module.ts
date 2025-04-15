import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BaocaodiemdanhquanlycosoComponent } from './baocaodiemdanhquanlycoso.component';

const routes: Routes = [
  {
      path: '',
       component: BaocaodiemdanhquanlycosoComponent,
      data: {
        title: 'Bao cao diem danh quan ly co so'
      }
    },
    
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BaocaodiemdanhquanlycosoRoutingModule { }
