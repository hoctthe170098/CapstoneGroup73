import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AccountmanagerComponent } from './quanly.component';

const routes: Routes = [
  {
    path: '',
     component: AccountmanagerComponent,
    data: {
      title: 'Nhân viên'
    }
  },
  {
    path: 'danh-sach-nhan-vien',
    component: AccountmanagerComponent
  },
  
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountmanagerRoutingModule { }

