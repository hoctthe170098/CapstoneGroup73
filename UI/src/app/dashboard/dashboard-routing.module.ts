import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { Dashboard1Component } from "./dashboard1/dashboard1.component";
import { Dashboard2Component } from "./dashboard2/dashboard2.component";
import { KehoachComponent } from './kehoach/kehoach.component';
import { ChuongtrinhComponent } from './chuongtrinh/chuongtrinh.component';
import { LoainhansuComponent } from './loainhansu/loainhansu.component';


const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: 'dashboard1',
        component: Dashboard1Component,
        data: {
          title: 'Dashboard 1'
        }
      },
      {
        path: 'dashboard2',
        component: Dashboard2Component,
        data: {
          title: 'Dashboard 2'
        }
      },
      {
        path: 'kehoach',
        component: KehoachComponent,
        data: {
          title: 'Kế hoạch'
        }
      },
      {
        path: 'chuongtrinh',
        component: ChuongtrinhComponent,
        data: {
          title: 'Chương trình'
        }
      },
      {
        path: 'loainhansu',
        component: LoainhansuComponent,
        data: {
          title: 'Loại nhân sự'
        }
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DashboardRoutingModule { }
