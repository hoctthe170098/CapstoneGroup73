import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TestlistRoutingModule } from './baikiemtra-routing.module';
import { TestListComponent } from './baikiemtra.component';


@NgModule({
  declarations: [
    TestListComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    TestlistRoutingModule
  ]
})
export class TestlistModule { }
