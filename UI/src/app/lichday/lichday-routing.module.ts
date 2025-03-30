import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LichDayComponent } from './lichday.component';

const routes: Routes = [
  {
    path: '',
     component: LichDayComponent,
    data: {
      title: 'Lịch Dạy'
    }
  },
  {
    path: 'danh-sach-lich-day',
    component: LichDayComponent
  },
  
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LicdayRoutingModule { }

