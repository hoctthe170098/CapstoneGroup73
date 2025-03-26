import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TestListComponent } from './testlist.component';


const routes: Routes = [
  {
      path: '',
       component: TestListComponent,
      data: {
        title: 'Test list'
      }
    },
    
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TestlistRoutingModule { }
