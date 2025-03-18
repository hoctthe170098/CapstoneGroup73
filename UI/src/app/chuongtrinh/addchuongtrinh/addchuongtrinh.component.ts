import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ChuongtrinhService } from '../shared/chuongtrinh.service';

@Component({
  selector: 'app-addchuongtrinh',
  templateUrl: './addchuongtrinh.component.html',
  styleUrls: ['./addchuongtrinh.component.scss']
})
export class AddchuongtrinhComponent {
  program: any = {
    tieuDe: '',
    moTa: '',
    noiDungBaiHocs: [
      {
        tieuDe: '',
        mota: '',
        soThuTu: 1,
        taiLieuHocTaps: [],
        expanded: false
      }
    ]
  };

  constructor(
    private router: Router,
    private chuongtrinhService: ChuongtrinhService
  ) {}

  /** ✅ Thêm bài học mới */
  addLesson() {
    const soThuTu = this.program.noiDungBaiHocs.length + 1;
    this.program.noiDungBaiHocs.push({
      tieuDe: '',
      mota: '',
      soThuTu,
      taiLieuHocTaps: [],
      expanded: false
    });
  }

  /** ✅ Xóa bài học */
  removeLesson(index: number) {
    this.program.noiDungBaiHocs.splice(index, 1);
    // Cập nhật lại số thứ tự
    this.program.noiDungBaiHocs.forEach((lesson, i) => {
      lesson.soThuTu = i + 1;
    });
  }

  /** ✅ Toggle mở rộng nội dung bài học */
  toggleLesson(index: number) {
    this.program.noiDungBaiHocs[index].expanded = !this.program.noiDungBaiHocs[index].expanded;
  }

  /** ✅ Xử lý tải file lên (chỉ lưu tạm, không upload ngay) */
  onFileChange(event: Event, lessonIndex: number) {
    const files = (event.target as HTMLInputElement).files;
    if (files) {
      for (let i = 0; i < files.length; i++) {
        this.addFileToLesson(files[i], lessonIndex);
      }
    }
  }

  /** ✅ Xử lý kéo thả file */
  onDragOver(event: DragEvent) {
    event.preventDefault();
  }

  onDrop(event: DragEvent, lessonIndex: number) {
    event.preventDefault();
    if (event.dataTransfer?.files.length) {
      for (let i = 0; i < event.dataTransfer.files.length; i++) {
        this.addFileToLesson(event.dataTransfer.files[i], lessonIndex);
      }
    }
  }

  /** ✅ Lưu file vào danh sách bài học (Không upload ngay) */
  addFileToLesson(file: File, lessonIndex: number) {
    const allowedTypes = ['application/pdf', 'video/mp4', 'application/msword'];
    if (!allowedTypes.includes(file.type)) {
      alert('Chỉ chấp nhận file .pdf, .mp4, .doc!');
      return;
    }

    this.program.noiDungBaiHocs[lessonIndex].taiLieuHocTaps.push({
      urlType: file.type.includes('video') ? 'video' : 'pdf',
      file
    });
  }

  /** ✅ Xóa file khỏi danh sách */
  removeFile(lessonIndex: number, fileIndex: number) {
    this.program.noiDungBaiHocs[lessonIndex].taiLieuHocTaps.splice(fileIndex, 1);
  }

  /** ✅ Gửi chương trình lên API (bao gồm cả file) */
  saveProgram() {
    const formData = new FormData();

    formData.append('chuongTrinhDto.tieuDe', this.program.tieuDe);
    formData.append('chuongTrinhDto.moTa', this.program.moTa);

    this.program.noiDungBaiHocs.forEach((lesson, i) => {
      formData.append(`chuongTrinhDto.noiDungBaiHocs[${i}].tieuDe`, lesson.tieuDe);
      formData.append(`chuongTrinhDto.noiDungBaiHocs[${i}].mota`, lesson.mota);
      formData.append(`chuongTrinhDto.noiDungBaiHocs[${i}].soThuTu`, lesson.soThuTu.toString());

      lesson.taiLieuHocTaps.forEach((file, j) => {
        formData.append(`chuongTrinhDto.noiDungBaiHocs[${i}].taiLieuHocTaps[${j}].urlType`, file.urlType);
        formData.append(`chuongTrinhDto.noiDungBaiHocs[${i}].taiLieuHocTaps[${j}].file`, file.file);
      });
    });

    this.chuongtrinhService.addProgram(formData).subscribe({
      next: (response) => {
        console.log('✅ Thêm chương trình thành công:', response);
        alert('Thêm chương trình thành công!');
        this.router.navigate(['/chuongtrinh']);
      },
      error: (error) => {
        console.error('❌ Lỗi khi thêm chương trình:', error);
        alert('Lỗi khi thêm chương trình!');
      }
    });
  }

  /** ✅ Hủy thêm */
  cancel() {
    this.router.navigate(['/chuongtrinh']);
  }
}
