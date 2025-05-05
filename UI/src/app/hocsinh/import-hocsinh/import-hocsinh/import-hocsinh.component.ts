import { Component, ChangeDetectorRef } from '@angular/core';
import { HocSinhService } from 'app/hocsinh/shared/hocsinh.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
@Component({
  selector: 'app-import-hocsinh',
  templateUrl: './import-hocsinh.component.html',
  styleUrls: ['./import-hocsinh.component.scss']
})
export class ImportHocsinhComponent {
  fileSelected: boolean = false;
  fileName: string = '';
  importedStudents: any[] = [];

  constructor(
    private hocSinhService: HocSinhService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef,
    private router: Router,
    private spinner: NgxSpinnerService
  ) {}

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.fileName = file.name;
      this.importFile(file);
    }
  }

  onDropFile(event: DragEvent) {
    event.preventDefault();
    if (event.dataTransfer?.files.length) {
      const file = event.dataTransfer.files[0];
      this.fileName = file.name;
      this.importFile(file);
    }
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
  }

  importFile(file: File) {
    this.hocSinhService.importHocSinhsFromExcel(file).subscribe({
      next: (res) => {
        if (!res.isError && res.data) {
          this.importedStudents = res.data.map((s: any) => ({
            ...s,
            showDetails: false,
            status: s.isActive ? 'Hoạt động' : 'Không hoạt động',
            chinhSach: s.tenChinhSach || 'Cơ bản',
            chinhSachId: s.chinhSachId || null,
            lopHocs: s.tenLops || []
          }));
          this.fileSelected = true;
          this.toastr.success('Import thành công!');
          this.cdr.detectChanges();
        } else {
          this.toastr.error(res.errors || 'Import thất bại!');
        }
      },
      error: (err) => {
        console.error(" Import lỗi:", err);
        this.toastr.error('Đã xảy ra lỗi khi import!');
      }
    });
  }

  toggleDetails(index: number) {
    this.importedStudents[index].showDetails = !this.importedStudents[index].showDetails;
  }

  onCancel() {
    this.fileSelected = false;
    this.fileName = '';
    this.importedStudents = [];
  }

  onConfirmImport() {
    const body = {
      hocSinhs: this.importedStudents.map(s => ({
        code: s.code,
        ten: s.ten,
        gioiTinh: s.gioiTinh,
        diaChi: s.diaChi,
        lop: s.lop,
        truongDangHoc: s.truongDangHoc,
        ngaySinh: s.ngaySinh,
        email: s.email,
        soDienThoai: s.soDienThoai,
        chinhSachId: s.chinhSachId || null,
      }))
    };
    this.spinner.show();
    this.hocSinhService.addListHocSinhs(body).subscribe({
      next: (res) => {
        this.spinner.hide();
        if (!res.isError) {
          this.toastr.success(res.message || 'Thêm học sinh thành công!');
          this.router.navigate(['/hocsinh']);
        } else {
          const errorMsg = res.errors?.join(', ') || res.message || 'Đã xảy ra lỗi khi thêm!';
          this.toastr.error(errorMsg, 'Lỗi');
        }
      },
      error: (err) => {
        this.spinner.hide(); 
        console.error(" Lỗi khi gọi API thêm học sinh:", err);
        this.toastr.error('Không thể thêm học sinh. Vui lòng thử lại!', 'Lỗi');
      }
    });
  }

  goBack() {
    window.history.back();
  }
}
