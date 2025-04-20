import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardCMComponent } from './dashboard-cm.component';
import { DashboardCMRoutingModule } from './dashboard-cm-routing.module';
import { ChartsModule } from 'ng2-charts';

@NgModule({
  declarations: [
    DashboardCMComponent
  ],
  imports: [
    CommonModule,
    ChartsModule,
    DashboardCMRoutingModule
  ]
})
export class DashboardCMModule { }
