import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CosoComponent } from './coso.component';


const routes: Routes = [
  {
    path: '',
     component: CosoComponent,
    data: {
      title: 'Cơ sở'
    }
  },
  {
    path: 'danh-sach-coso',
    component: CosoComponent
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CosoRoutingModule { }
