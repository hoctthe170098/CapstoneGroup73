import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AccountmanagerComponent } from './accountmanager.component';

const routes: Routes = [
  {
    path: '',
     component: AccountmanagerComponent,
    data: {
      title: 'Tài Khoản'
    }
  },
  {
    path: 'danh-sach-tai-khoan',
    component: AccountmanagerComponent
  },
  
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountmanagerRoutingModule { }

