import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HocsinhComponent } from './hocsinh.component';
import { ImportHocsinhComponent } from './import-hocsinh/import-hocsinh/import-hocsinh.component';

const routes: Routes = [
  {
    path: '',
     component: HocsinhComponent,
    data: {
      title: 'Học Sinh'
    }
  },
  {
    path: 'danh-sach-hoc-sinh',
    component: HocsinhComponent
  },
  { path: 'import-hocsinh', 
    component: ImportHocsinhComponent 
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HocSinhRoutingModule { }
