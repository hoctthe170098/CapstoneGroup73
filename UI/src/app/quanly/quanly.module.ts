import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AccountmanagerRoutingModule } from './quanly-routing.module';
import { AccountmanagerComponent } from './quanly.component';
import { AccountmanagerService } from './shared/quanly.service';


@NgModule({
  declarations: [
    AccountmanagerComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AccountmanagerRoutingModule
  ],
  exports: [
    AccountmanagerRoutingModule
      ],
  providers: [
    AccountmanagerService 
  ]
})
export class AccountmanagerModule { }
