import { ChangeDetectorRef, Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { DatatableData, pageSizeOptions } from './data/chuongtrinhModel';
import { ColumnMode, DatatableComponent } from '@swimlane/ngx-datatable';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { CreatechuongtrinhComponent } from './createchuongtrinh/createchuongtrinh.component';
import { Chuongtrinh } from './shared/chuongtrinh.model';
import { ToastrService } from 'ngx-toastr';
import { ChuongtrinhService } from './shared/chuongtrinh.service';
import { UpdatechuongtrinhComponent } from './updatechuongtrinh/updatechuongtrinh.component';
import { DeletechuongtrinhComponent } from './deletechuongtrinh/deletechuongtrinh.component';

@Component({
  selector: 'app-chuongtrinh',
  templateUrl: './chuongtrinh.component.html',
  styleUrls: ['./chuongtrinh.component.scss', '../../../assets/sass/libs/datatables.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ChuongtrinhComponent implements OnInit {
  page = {
    Search: '',
    PageNumber: 1,
    PageSize: 2
  }
  data: Chuongtrinh;
  total: number = 0;
  offset: number = 0;
  messages = {
    emptyMessage: 'Không có dữ liệu để hiển thị',
    totalMessage: 'bản ghi',
    selectedMessage: 'đã chọn'
  }
  constructor(private dialog: MatDialog, private service: ChuongtrinhService, private toastr: ToastrService,private cdr: ChangeDetectorRef ) { }
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
    this.getChuongTrinh();
  }
  getChuongTrinh() {
    this.service.getChuongTrinhList(this.page).subscribe((res: any) => {
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
    const index = this.rows.indexOf(row); // Lấy index thực tế của hàng
    return index + 1 + this.page.PageSize * (this.page.PageNumber - 1); // Trả về index + 1
  }
  updateLimit(event: any) {
    this.page.PageSize = event.target.value;
    this.page.PageNumber = 1;
    this.getChuongTrinh();
  }
  Search(event: any) {
    this.page.Search = event.target.value;
    this.page.PageNumber = 1;
    this.getChuongTrinh();
  }
  setPage(pageInfo) {
    this.page.PageNumber = pageInfo.offset + 1;
    this.getChuongTrinh();
  }
  create() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.autoFocus = true;
    dialogConfig.disableClose = false;
    dialogConfig.width = "70%";
    this.dialog.open(CreatechuongtrinhComponent, dialogConfig).afterClosed().subscribe(res => {
      if (this.service.isCreateOrUpdateOrDelete == true) {
        this.page.PageNumber = 1;
        this.service.isCreateOrUpdateOrDelete = false;
        this.getChuongTrinh();
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
    this.data = this.rows.find((k: Chuongtrinh) => k.id == id);
    console.log(this.data)
    dialogConfig.data = this.data;
    this.dialog.open(UpdatechuongtrinhComponent, dialogConfig).afterClosed().subscribe(res => {
      if (this.service.isCreateOrUpdateOrDelete == true) {
        this.page.PageNumber = 1;
        this.service.isCreateOrUpdateOrDelete = false;
        this.getChuongTrinh();
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
    this.dialog.open(DeletechuongtrinhComponent, dialogConfig).afterClosed().subscribe(res => {
      if (this.service.isCreateOrUpdateOrDelete == true) {
        this.page.PageNumber = 1;
        this.service.isCreateOrUpdateOrDelete = false;
        this.getChuongTrinh();
      }
    });
  }
}
