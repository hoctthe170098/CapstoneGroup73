import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GiaovienComponent } from './giaovien.component';
import { ImportGiaovienComponent } from './import-giaovien/import-giaovien.component';
const routes: Routes = [
  {
    path: '',
     component: GiaovienComponent,
    data: {
      title: 'Giáo viên'
    }
  },
  {
    path: 'danh-sach-giao-vien',
    component: GiaovienComponent
  },
  {
    path: 'import-giao-vien',
    component: ImportGiaovienComponent
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GiaovienRoutingModule { }

