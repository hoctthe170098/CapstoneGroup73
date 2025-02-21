import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SanphamComponent } from './sanpham.component';
import { CreatesanphamComponent } from './createsanpham/createsanpham.component';

const routes: Routes = [
  {
    path: '',
     component: SanphamComponent,
    data: {
      title: 'Sản phẩm'
    }
  },
  {
    path: 'tao-moi-san-pham',
    component: CreatesanphamComponent
  },
  {
    path: 'danh-sach-san-pham',
    component: SanphamComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SanphamRoutingModule { }
