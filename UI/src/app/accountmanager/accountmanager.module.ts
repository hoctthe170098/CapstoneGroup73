import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AccountmanagerRoutingModule } from './accountmanager-routing.module';
import { AccountmanagerComponent } from './accountmanager.component';
import { AccountmanagerService } from './shared/account.service';


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
