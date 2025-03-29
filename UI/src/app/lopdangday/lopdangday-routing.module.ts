import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LopdangdayComponent } from './lopdangday.component';

const routes: Routes = [
  {
    path: '',
    component: LopdangdayComponent,
    data: {
      title: 'lopdangday'
    }
  }
    
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LopdangdayRoutingModule { }
