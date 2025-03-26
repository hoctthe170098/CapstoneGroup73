import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChinhsachComponent } from './chinhsach.component';
const routes: Routes = [
  {
    path: '',
    component: ChinhsachComponent,
    data: {
      title: 'phong'
    }
  },
  {
    path: 'danh-sach-phong',
    component: ChinhsachComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ChinhsachRoutingModule { }
