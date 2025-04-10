import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LopdanghocComponent } from './lopdanghoc.component';

const routes: Routes = [
  {
        path: '',
         component: LopdanghocComponent,
        data: {
          title: 'Lớp đang học'
        }
      },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})

export class LopdanghocRoutingModule { }
