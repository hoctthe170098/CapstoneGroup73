import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SanphamRoutingModule } from './sanpham-routing.module';
import { SanphamComponent } from './sanpham.component';
import { ChartistModule } from 'ng-chartist';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MatchHeightModule } from 'app/shared/directives/match-height.directive';
import { NgApexchartsModule } from 'ng-apexcharts';
import { AngularResizedEventModule } from 'angular-resize-event';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ToastrModule } from 'ngx-toastr';
import { CreatesanphamComponent } from './createsanpham/createsanpham.component';
import { MatDialogModule } from '@angular/material/dialog';
import { QuillModule } from 'ngx-quill';


@NgModule({
  declarations: [
    SanphamComponent,
    CreatesanphamComponent 
  ],
  imports: [
    CommonModule,
    MatDialogModule,
    SanphamRoutingModule,
    ChartistModule,
    NgbModule,
    MatchHeightModule,
    NgApexchartsModule,
    AngularResizedEventModule,
    NgxDatatableModule,
    FormsModule,
    ReactiveFormsModule,
    ToastrModule,
    QuillModule.forRoot({
      modules: {
        toolbar: [
          ['bold', 'italic', 'underline', 'strike'],        // toggled buttons
          ['blockquote', 'code-block'],
      
          [{ 'header': 1 }, { 'header': 2 }],               // custom button values
          [{ 'list': 'ordered'}, { 'list': 'bullet' }],
          [{ 'script': 'sub'}, { 'script': 'super' }],      // superscript/subscript
          [{ 'indent': '-1'}, { 'indent': '+1' }],          // outdent/indent
          [{ 'direction': 'rtl' }],                         // text direction
      
          [{ 'size': ['small', false, 'large', 'huge'] }],  // custom dropdown
          [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
      
          [{ 'color': [] }, { 'background': [] }],          // dropdown with defaults from theme
          [{ 'font': [] }],
          [{ 'align': [] }],   
          ['clean']                                        // remove formatting button
        ]
      }
    }),
    
  ]
})
export class SanphamModule { }
