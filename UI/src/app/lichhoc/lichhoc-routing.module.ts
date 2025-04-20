import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LichhocComponent } from './lichhoc.component';
const routes: Routes = [
  {
    path: '',
     component: LichhocComponent,
    data: {
      title: 'Lịch Học'
    }
  },
  {
    path: 'danh-sach-lich-hoc',
    component: LichhocComponent
  },
  
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LichhocRoutingModule { }
