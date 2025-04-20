import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardCMComponent } from './dashboard-cm.component';

const routes: Routes = [
  {
    path: '',
     component: DashboardCMComponent,
    data: {
      title: 'dashboard-cm'
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardCMRoutingModule { }
