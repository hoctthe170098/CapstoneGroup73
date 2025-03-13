import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChuongtrinhComponent } from './chuongtrinh.component';
import { EditchuongtrinhComponent } from './editchuongtrinh/editchuongtrinh.component';
import { AddchuongtrinhComponent } from './addchuongtrinh/addchuongtrinh.component';

const routes: Routes = [
  {
    path: '',
    component: ChuongtrinhComponent,
    data: {
      title: 'Chương Trình'
    }
  },
  {
    path: 'danh-sach-chuongtrinh',
    component: ChuongtrinhComponent
  },
  {
    path: 'add',
    component: AddchuongtrinhComponent,
    data: {
      title: 'Thêm Chương Trình'
    }
  },
  {
    path: 'edit/:id',
    component: EditchuongtrinhComponent,
    data: {
      title: 'Chỉnh Sửa Chương Trình'
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ChuongtrinhRoutingModule { }
