import { ChangeDetectorRef, Component } from '@angular/core';
import { GiaovienService } from '../shared/giaovien.service';
import { ToastrService } from 'ngx-toastr';

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
    private cdr: ChangeDetectorRef
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
    this.toastr.success('Đã xác nhận danh sách giáo viên!', 'Thành công');
    // 👇 Giữ nguyên danh sách và chuyển trạng thái về đã import
    this.importedTeachers.forEach(t => (t.status = 'Hoạt động'));
    this.cdr.detectChanges();
  }

  goBack() {
    window.history.back();
  }

  onEditStudentClick(index: number) {
    this.toastr.info('Tính năng sửa đang được phát triển!');
  }
}
