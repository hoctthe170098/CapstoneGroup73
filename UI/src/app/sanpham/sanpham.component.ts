import { ChangeDetectorRef, Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { ColumnMode, DatatableComponent } from '@swimlane/ngx-datatable';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { SanPham, pageSizeOptions } from './shared/sanpham.model';
import { SanPhamService } from './shared/sanpham.service';

@Component({
  selector: 'app-sanpham',
  templateUrl: './sanpham.component.html',
  styleUrls: ['./sanpham.component.scss', './../../assets/sass/libs/datatables.scss'],
  encapsulation: ViewEncapsulation.None
})
export class SanphamComponent implements OnInit {

  page = {
    Search: '',
    PageNumber: 1,
    PageSize: 2
  }
  data: SanPham;
  total: number = 0;
  offset: number = 0;
  messages = {
    emptyMessage: 'Không có dữ liệu để hiển thị',
    totalMessage: 'bản ghi',
    selectedMessage: 'đã chọn'
  }
  constructor(private dialog: MatDialog, private service: SanPhamService
    , private toastr: ToastrService,private cdr: ChangeDetectorRef,private router:Router ) { }
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
    this.getSanPham();
  }
  getSanPham() {
    this.service.getSanPhamList(this.page).subscribe((res: any) => {
      if(!res.IsError){
        this.rows = res.data.items;
        this.columns = [
          { name: 'Code', prop: 'code' },
          { name: 'Name', prop: 'name' },
          { name: 'tenPhanLoaiCongViec', prop: 'tenPhanLoaiCongViec' },
          { name: 'description', prop: 'description' },
          { name: 'beginTime', prop: 'beginTime' },
          { name: 'endTime', prop: 'endTime' },
          { name: 'content', prop: 'content' },
          { name: 'tenGiaiDoanDuAn', prop: 'tenGiaiDoanDuAn' },
          { name: 'tenTrangThaiThucHien', prop: 'tenTrangThaiThucHien' },
          { name: 'tenMucDoRuiRo', prop: 'tenMucDoRuiRo' },
          { name: 'note', prop: 'note' }
        ]
        this.pageSizeOptions = pageSizeOptions;
        this.Limit = pageSizeOptions[0];
        this.total = res.data.totalCount;
        this.offset = res.data.pageNumber - 1;
        this.cdr.detectChanges();
      }else{
        this.toastr.error(res.message)
      }   
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
    this.getSanPham();
  }
  Search(event: any) {
    this.page.Search = event.target.value;
    this.page.PageNumber = 1;
    this.getSanPham();
  }
  setPage(pageInfo) {
    this.page.PageNumber = pageInfo.offset + 1;
    this.getSanPham();
  }
  create() {
    this.router.navigate(['/sanpham/tao-moi-san-pham']);
  }
  checkClassStatus(row) {
    if (row.status == 'Hoạt động') return true;
    return false;
  }
  // update(id: string) {
  //   const dialogConfig = new MatDialogConfig();
  //   dialogConfig.autoFocus = true;
  //   dialogConfig.disableClose = false;
  //   dialogConfig.width = "70%";
  //   this.data = this.rows.find((k: SanPham) => k.id == id);
  //   console.log(this.data)
  //   dialogConfig.data = this.data;
  //   this.dialog.open(UpdateSanPhamComponent, dialogConfig).afterClosed().subscribe(res => {
  //     if (this.service.isCreateOrUpdateOrDelete == true) {
  //       this.page.PageNumber = 1;
  //       this.service.isCreateOrUpdateOrDelete = false;
  //       this.getSanPham();
  //     }  
  //   });
  // }
  // delete(id: string) {
  //   const dialogConfig = new MatDialogConfig();
  //   dialogConfig.autoFocus = true;
  //   dialogConfig.disableClose = false;
  //   dialogConfig.width = "20%";
  //   dialogConfig.position = {
  //     top: '50px'
  //   }
  //   dialogConfig.data = id;
  //   this.dialog.open(DeleteSanPhamComponent, dialogConfig).afterClosed().subscribe(res => {
  //     if (this.service.isCreateOrUpdateOrDelete == true) {
  //       this.page.PageNumber = 1;
  //       this.service.isCreateOrUpdateOrDelete = false;
  //       this.getSanPham();
  //     }
  //   });
  // }

}
