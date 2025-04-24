import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardAdminComponent } from './dashboard-admin.component';
import { DashboardAdminRoutingModule } from './dashboard-admin-routing.module';
import { ChartsModule } from 'ng2-charts';


@NgModule({
  declarations: [DashboardAdminComponent],
  imports: [
      CommonModule,
      ChartsModule,
      DashboardAdminRoutingModule
    ]
})
export class DashboardAdminModule { }
