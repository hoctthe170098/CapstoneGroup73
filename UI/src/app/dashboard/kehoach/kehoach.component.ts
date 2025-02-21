import { Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef } from '@angular/core';
import { DatatableData, pageSizeOptions } from './data/kehoachModel';
import {
  ColumnMode,
  DatatableComponent,
  SelectionType
} from '@swimlane/ngx-datatable';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { CreateComponent } from './create/create.component';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { ItemService } from './shared/kehoach.service';
import { KeHoach } from './shared/kehoach.model';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { UpdatekehoachComponent } from './updatekehoach/updatekehoach.component';
import { DeletekehoachComponent } from './deletekehoach/deletekehoach.component';

@Component({
  selector: 'app-kehoach',
  templateUrl: './kehoach.component.html',
  styleUrls: ['./kehoach.component.scss', '../../../assets/sass/libs/datatables.scss'],
  encapsulation: ViewEncapsulation.None
})
export class KehoachComponent implements OnInit {
  page = {
    Search: '',
    PageNumber: 1,
    PageSize: 2
  }
  data: KeHoach;
  total: number = 0;
  offset: number = 0;
  messages = {
    emptyMessage: 'Không có dữ liệu để hiển thị',
    totalMessage: 'bản ghi',
    selectedMessage: 'đã chọn'
  }
  constructor(private dialog: MatDialog, private service: ItemService, private toastr: ToastrService
    ,private cdr: ChangeDetectorRef) { }
  public rows: any[] = [];
  public columns: any[] = [];
  public multiPurposeRows : any[] = []
  public pageSizeOptions: any
  public Limit: number
  public ColumnMode = ColumnMode;
  @ViewChild(DatatableComponent) table: DatatableComponent;
  @ViewChild('tableRowDetails') tableRowDetails: any;
  @ViewChild('tableResponsive') tableResponsive: any;
  public contentHeader: object;
  ngOnInit(): void {
    this.getKeHoach();
  }
  getKeHoach() {
    this.service.getKehoachList(this.page).subscribe((res: any) => {
      this.rows = res.data.items;
      this.multiPurposeRows = this.rows;
      this.columns = [
        { name: 'Code', prop: 'code' },
        { name: 'Name', prop: 'name' },
        { name: 'Status', prop: 'status' }
      ]
      this.pageSizeOptions = pageSizeOptions;
      this.Limit = pageSizeOptions[0];
      this.total = res.data.totalCount;
      this.offset = res.data.pageNumber - 1;
      this.cdr.detectChanges();
    });
  }
  // row data
  // column header
  // multi Purpose datatable Row data
  

  rowDetailsToggleExpand(row) {
    this.tableRowDetails.rowDetail.toggleExpandRow(row);
  }
  getIndex(row: any): number {
    const index = this.rows.indexOf(row); 
    return index + 1 + this.page.PageSize * (this.page.PageNumber - 1); 
  }
  updateLimit(event: any) {
    this.page.PageSize = event.target.value;
    this.page.PageNumber = 1;
    this.getKeHoach();
  }
  Search(event: any) {
    this.page.Search = event.target.value;
    this.page.PageNumber = 1;
    this.getKeHoach();
  }
  setPage(pageInfo) {
    this.page.PageNumber = pageInfo.offset + 1;
    this.getKeHoach();
  }
  create() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.autoFocus = true;
    dialogConfig.disableClose = false;
    dialogConfig.width = "70%";
    this.dialog.open(CreateComponent, dialogConfig).afterClosed().subscribe(res => {
      if (this.service.isCreateOrUpdateOrDelete == true) {
        this.page.PageNumber = 1;
        this.service.isCreateOrUpdateOrDelete = false;
        this.getKeHoach();
      }  
    });
  }
  checkClassStatus(row) {
    if (row.status == 'Hoạt động') return true;
    return false;
  }
  update(id: string) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.autoFocus = true;
    dialogConfig.disableClose = false;
    dialogConfig.width = "70%";
    this.data = this.rows.find((k: KeHoach) => k.id == id);
    dialogConfig.data = this.data;
    this.dialog.open(UpdatekehoachComponent, dialogConfig).afterClosed().subscribe(res => {
      if (this.service.isCreateOrUpdateOrDelete == true) {
        this.page.PageNumber = 1;
        this.service.isCreateOrUpdateOrDelete = false;
        this.getKeHoach();
      }  
    });
  }
  delete(id: string) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.autoFocus = true;
    dialogConfig.disableClose = false;
    dialogConfig.width = "20%";
    dialogConfig.position = {
      top: '50px'
    }
    dialogConfig.data = id;
    this.dialog.open(DeletekehoachComponent, dialogConfig).afterClosed().subscribe(res => {
      if (this.service.isCreateOrUpdateOrDelete == true) {
        this.page.PageNumber = 1;
        this.service.isCreateOrUpdateOrDelete = false;
        this.getKeHoach();
      }
    });
  }
}
