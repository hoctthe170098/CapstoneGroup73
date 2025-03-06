import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChuongtrinhComponent } from './chuongtrinh.component';
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
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ChuongtrinhRoutingModule { }
