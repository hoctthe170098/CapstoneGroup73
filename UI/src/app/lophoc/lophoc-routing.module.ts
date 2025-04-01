import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LophocComponent } from './lophoc.component';
import { AddlophocComponent } from './addlophoc/addlophoc.component';
import { EditLopHocComponent } from './editlophoc/editlophoc.component';

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
     }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LophocRoutingModule { }
