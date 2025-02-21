import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { DashboardRoutingModule } from "./dashboard-routing.module";
import { ChartistModule } from 'ng-chartist';
import { NgApexchartsModule } from 'ng-apexcharts';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AngularResizedEventModule } from 'angular-resize-event';
import { MatchHeightModule } from "../shared/directives/match-height.directive";
import { Dashboard1Component } from "./dashboard1/dashboard1.component";
import { Dashboard2Component } from "./dashboard2/dashboard2.component";
import { KehoachComponent } from './kehoach/kehoach.component';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { CreateComponent } from './kehoach/create/create.component';
import {MatDialogModule} from '@angular/material/dialog';
import { ReactiveFormsModule,FormsModule } from '@angular/forms';
import { ChuongtrinhComponent } from './chuongtrinh/chuongtrinh.component';
import { LoainhansuComponent } from './loainhansu/loainhansu.component';
import { CreatechuongtrinhComponent } from './chuongtrinh/createchuongtrinh/createchuongtrinh.component';
import { CreateloainhansuComponent } from './loainhansu/createloainhansu/createloainhansu.component';
import { ToastrModule } from 'ngx-toastr';
import { UpdatekehoachComponent } from './kehoach/updatekehoach/updatekehoach.component';
import { DeletekehoachComponent } from './kehoach/deletekehoach/deletekehoach.component';
import { UpdatechuongtrinhComponent } from './chuongtrinh/updatechuongtrinh/updatechuongtrinh.component';
import { DeletechuongtrinhComponent } from './chuongtrinh/deletechuongtrinh/deletechuongtrinh.component';
import { UpdateloainhansuComponent } from './loainhansu/updateloainhansu/updateloainhansu.component';
import { DeleteloainhansuComponent } from './loainhansu/deleteloainhansu/deleteloainhansu.component';
@NgModule({
    imports: [
        CommonModule,
        MatDialogModule,
        DashboardRoutingModule,
        ChartistModule,
        NgbModule,
        MatchHeightModule,
        NgApexchartsModule,
        AngularResizedEventModule,
        NgxDatatableModule,
        FormsModule,
        ReactiveFormsModule,
        ToastrModule
    ],
    exports: [],
    declarations: [
        Dashboard1Component,
        Dashboard2Component,
        KehoachComponent,
        CreateComponent,
        ChuongtrinhComponent,
        LoainhansuComponent,
        CreatechuongtrinhComponent,
        CreateloainhansuComponent,
        UpdatekehoachComponent,
        DeletekehoachComponent,
        UpdatechuongtrinhComponent,
        DeletechuongtrinhComponent,
        UpdateloainhansuComponent,
        DeleteloainhansuComponent
    ],
    providers: [],
})
export class DashboardModule { }
