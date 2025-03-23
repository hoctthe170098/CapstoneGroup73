import { Component, ChangeDetectorRef } from '@angular/core';
import { HocSinhService } from 'app/hocsinh/shared/hocsinh.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-import-hocsinh',
  templateUrl: './import-hocsinh.component.html',
  styleUrls: ['./import-hocsinh.component.scss']
})
export class ImportHocsinhComponent {
  fileSelected: boolean = false;
  fileName: string = '';
  importedStudents: any[] = [];

  constructor(private hocSinhService: HocSinhService, private toastr: ToastrService,private cdr: ChangeDetectorRef) {}

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
            lopHocs: s.tenLops || []
          }));
          this.fileSelected = true;
          this.toastr.success('Import thành công!');
          this.cdr.detectChanges();
        } else {
          this.toastr.error(res.message || 'Import thất bại!');
        }
      },
      error: (err) => {
        console.error("❌ Import lỗi:", err);
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
    alert('✅ Đã xác nhận import file: ' + this.fileName);
    // TODO: Gọi API lưu danh sách này nếu có
  }

  goBack() {
    window.history.back();
  }
}
