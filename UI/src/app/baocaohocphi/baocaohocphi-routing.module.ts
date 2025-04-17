import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BaocaohocphiComponent } from './baocaohocphi.component';

const routes: Routes = [
  {
      path: '',
       component: BaocaohocphiComponent,
      data: {
        title: 'Bao cao hoc phi'
      }
    },
    
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BaocaohocphiRoutingModule { }
