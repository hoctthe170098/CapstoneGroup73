import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LophocComponent } from './lophoc.component';

const routes: Routes = [
  {
      path: '',
       component: LophocComponent,
      data: {
        title: 'Lớp Học'
      }
    },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LophocRoutingModule { }
