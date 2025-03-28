import { ChangeDetectorRef, Component } from '@angular/core';
import { GiaovienService } from '../shared/giaovien.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Component({
  selector: 'app-import-giaovien',
  templateUrl: './import-giaovien.component.html',
  styleUrls: ['./import-giaovien.component.scss']
})
export class ImportGiaovienComponent {
  fileSelected: boolean = false;
  fileName: string = '';
  importedTeachers: any[] = [];

  constructor(
    private giaovienService: GiaovienService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef,
    private router: Router
  ) {}

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.fileName = file.name;
      this.fileSelected = true;

      this.giaovienService.importGiaoViensFromExcel(file).subscribe({
        next: (res) => {
          if (!res.isError && res.data) {
            this.importedTeachers = res.data.map((gv: any) => ({
              code: gv.code,
              ten: gv.ten,
              gioiTinh: gv.gioiTinh,
              ngaySinh: new Date(gv.ngaySinh),
              email: gv.email,
              soDienThoai: gv.soDienThoai,
              diaChi: gv.diaChi,
              truongDangDay: gv.truongDangDay,
              coso: gv.tenCoSo || 'Chưa có',
              lopHocs: gv.tenLops || [],
              status: gv.isActive ? 'Hoạt động' : 'Tạm ngừng',
              showDetails: false
            }));
            this.toastr.success('Import thành công!', 'Thành công');
            this.cdr.detectChanges();
          } else {
            this.toastr.error('Import thất bại, dữ liệu không hợp lệ', 'Lỗi');
            this.fileSelected = false;
          }
        },
        error: (err) => {
          console.error("❌ Import lỗi:", err);
          this.toastr.error('Không thể import file', 'Lỗi');
          this.fileSelected = false;
        }
      });
    }
  }

  onDropFile(event: DragEvent) {
    event.preventDefault();
    if (event.dataTransfer?.files.length) {
      const file = event.dataTransfer.files[0];
      this.onFileSelected({ target: { files: [file] } });
    }
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
  }

  toggleDetails(index: number) {
    this.importedTeachers[index].showDetails = !this.importedTeachers[index].showDetails;
  }

  onCancel() {
    this.fileSelected = false;
    this.fileName = '';
    this.importedTeachers = [];
  }

  onConfirmImport() {
    const danhSachGuiLen = this.importedTeachers.map(gv => ({
      code: gv.code,
      ten: gv.ten,
      gioiTinh: gv.gioiTinh,
      diaChi: gv.diaChi,
      truongDangDay: gv.truongDangDay,
      ngaySinh: gv.ngaySinh.toISOString().split('T')[0],
      email: gv.email,
      soDienThoai: gv.soDienThoai
    }));
  
    this.giaovienService.addListGiaoViens(danhSachGuiLen).subscribe({
      next: (res) => {
        if (!res.isError) {
          this.toastr.success(res.message || 'Đã thêm giáo viên vào hệ thống!', 'Thành công');
          this.router.navigate(['/giaovien']); 
        } else {
          const errorMessage = res.errors?.length ? res.errors.join(', ') : (res.message || 'Thêm giáo viên thất bại');
          this.toastr.error(errorMessage, 'Lỗi');
        }
      },
      error: (err) => {
        console.error(' Lỗi khi gọi API thêm danh sách giáo viên:', err);
        this.toastr.error('Không thể thêm giáo viên vào hệ thống.', 'Lỗi');
      }
    });
  }
  
  
  

  goBack() {
    window.history.back();
  }

  
}
