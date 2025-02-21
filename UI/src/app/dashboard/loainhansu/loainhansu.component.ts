import { ChangeDetectorRef, Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { DatatableData, pageSizeOptions } from './data/loainhansuModel';
import { ColumnMode, DatatableComponent } from '@swimlane/ngx-datatable';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { CreateloainhansuComponent } from './createloainhansu/createloainhansu.component';
import { LoaiNhanSu } from './shared/loainhansu.model';
import { LoaiNhanSuService } from './shared/loainhansu.service';
import { ToastrService } from 'ngx-toastr';
import { UpdateloainhansuComponent } from './updateloainhansu/updateloainhansu.component';
import { DeleteloainhansuComponent } from './deleteloainhansu/deleteloainhansu.component';

@Component({
  selector: 'app-loainhansu',
  templateUrl: './loainhansu.component.html',
  styleUrls: ['./loainhansu.component.scss', '../../../assets/sass/libs/datatables.scss'],
  encapsulation: ViewEncapsulation.None
})
export class LoainhansuComponent implements OnInit {
  page = {
    Search: '',
    PageNumber: 1,
    PageSize: pageSizeOptions[0]
  }
  data: LoaiNhanSu;
  total: number = 0;
  offset: number = 0;
  messages = {
    emptyMessage: 'Không có dữ liệu để hiển thị',
    totalMessage: 'bản ghi',
    selectedMessage: 'đã chọn'
  }
  constructor(private dialog: MatDialog, private service: LoaiNhanSuService, private toastr: ToastrService
    ,private cdr: ChangeDetectorRef) { }
  public rows: any[] = [];
  public columns: any[] = [];
  public pageSizeOptions: any
  public Limit: number
  public ColumnMode = ColumnMode;
  @ViewChild(DatatableComponent) table: DatatableComponent;
  @ViewChild('tableRowDetails') tableRowDetails: any;
  @ViewChild('tableResponsive') tableResponsive: any;
  public contentHeader: object;
  ngOnInit(): void {
    this.getLoaiNhanSu();
  }
  getLoaiNhanSu() {
    this.service.getLoainhansuList(this.page).subscribe((res: any) => {
      this.rows = res.data.items;
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
  //public multiPurposeRows = this.rows;

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
    this.getLoaiNhanSu();
  }
  Search(event: any) {
    this.page.Search = event.target.value;
    this.page.PageNumber = 1;
    this.getLoaiNhanSu();
  }
  setPage(pageInfo) {
    this.page.PageNumber = pageInfo.offset + 1;
    this.getLoaiNhanSu();
  }
  create() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.autoFocus = true;
    dialogConfig.disableClose = false;
    dialogConfig.width = "70%";
    this.dialog.open(CreateloainhansuComponent, dialogConfig).afterClosed().subscribe(res => {
      if (this.service.isCreateOrUpdateOrDelete == true) {
        this.page.PageNumber = 1;
        this.service.isCreateOrUpdateOrDelete = false;
        this.getLoaiNhanSu();
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
    this.data = this.rows.find((k: LoaiNhanSu) => k.id == id);
    console.log(this.data)
    dialogConfig.data = this.data;
    this.dialog.open(UpdateloainhansuComponent, dialogConfig).afterClosed().subscribe(res => {
      if (this.service.isCreateOrUpdateOrDelete == true) {
        this.page.PageNumber = 1;
        this.service.isCreateOrUpdateOrDelete = false;
        this.getLoaiNhanSu();
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
    this.dialog.open(DeleteloainhansuComponent, dialogConfig).afterClosed().subscribe(res => {
      if (this.service.isCreateOrUpdateOrDelete == true) {
        this.page.PageNumber = 1;
        this.service.isCreateOrUpdateOrDelete = false;
        this.getLoaiNhanSu();
      }  
    });
  }
}
