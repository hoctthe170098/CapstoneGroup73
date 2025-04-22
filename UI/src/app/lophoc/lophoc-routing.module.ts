import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LophocComponent } from './lophoc.component';
import { AddlophocComponent } from './addlophoc/addlophoc.component';
import { EditLopHocComponent } from './editlophoc/editlophoc.component';
import { BaocaodiemdanhquanlycosoComponent } from './baocaodiemdanhquanlycoso/baocaodiemdanhquanlycoso.component';
import { BaocaohocphiComponent } from './baocaohocphi/baocaohocphi.component';


const routes: Routes = [
    {
      path: '',
       component: LophocComponent,
      data: {
        title: 'Lớp Học'
      }
    },
    {
        path: 'add',
        component: AddlophocComponent,
        data: {
          title: 'Thêm Lớp Học'
        }
    },
    {
       path: 'edit/:tenLop',
       component: EditLopHocComponent,
       data: {
         title: 'Chỉnh Sửa Lớp Học'
       }
    },
    {
      path: 'baocaodiemdanhquanlycoso',
      component: BaocaodiemdanhquanlycosoComponent,
      data: {
        title: 'Báo Cáo Điểm Danh'
      }
    },
    {
      path: 'baocaohocphi',
      component: BaocaohocphiComponent,
      data: {
        title: 'Báo Cáo Học Phí'
      }
    }
     
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LophocRoutingModule { }
