import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ChuongtrinhService } from '../shared/chuongtrinh.service';
import { ToastrService } from 'ngx-toastr';
import { NgxSpinnerService } from 'ngx-spinner';
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

  errors: any = {
    tieuDe: '',
    moTa: '',
    noiDungBaiHocs: []
  };

  draggedLessonIndex: number | null = null;

  constructor(
    private router: Router,
    private chuongtrinhService: ChuongtrinhService,
    private toastr: ToastrService,
    private spinner: NgxSpinnerService
  ) {}
  ngOnInit(): void {
    
  }

  validateField(field: string) {
    const maxLength = field === 'tieuDe' ? 200 : 300;

    if (!this.program[field]?.trim()) {
      this.errors[field] = 'Trường này không được để trống!';
    } else if (this.program[field].length > maxLength) {
      this.errors[field] = ` Quá số ký tự cho phép, vui lòng nhập lại! Giới hạn: ${maxLength} ký tự.`;
    } else {
      this.errors[field] = '';
    }
  }

  /** ✅ Kiểm tra nội dung bài học */
  validateLesson(index: number, field: string) {
    if (!this.errors.noiDungBaiHocs[index]) {
      this.errors.noiDungBaiHocs[index] = {};
    }

    const maxLength = field === 'tieuDe' ? 200 : 300;
    const value = this.program.noiDungBaiHocs[index][field]?.trim();

    if (!value) {
      this.errors.noiDungBaiHocs[index][field] = 'Trường này không được để trống!';
    } else if (value.length > maxLength) {
      this.errors.noiDungBaiHocs[index][field] = `Quá số ký tự cho phép, vui lòng nhập lại! Giới hạn: ${maxLength} ký tự.`;
    } else {
      this.errors.noiDungBaiHocs[index][field] = '';
    }
  }

  /** ✅ Kiểm tra toàn bộ dữ liệu trước khi gửi */
  isFormValid() {
    let valid = true;

    ['tieuDe', 'moTa'].forEach(field => {
      this.validateField(field);
      if (this.errors[field]) valid = false;
    });

    this.errors.noiDungBaiHocs = [];
    this.program.noiDungBaiHocs.forEach((lesson, index) => {
      this.errors.noiDungBaiHocs[index] = {};
      ['tieuDe', 'mota'].forEach(field => {
        this.validateLesson(index, field);
        if (this.errors.noiDungBaiHocs[index][field]) valid = false;
      });
    });

    return valid;
  }
  

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
  onDragStart(event: DragEvent, index: number) {
    this.draggedLessonIndex = index;
    event.dataTransfer?.setData("text/plain", index.toString());
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
  }

  onDropLesson(event: DragEvent, targetIndex: number) {
    event.preventDefault();
    if (this.draggedLessonIndex === null || this.draggedLessonIndex === targetIndex) return;

    const movedLesson = this.program.noiDungBaiHocs[this.draggedLessonIndex];
    this.program.noiDungBaiHocs.splice(this.draggedLessonIndex, 1);
    this.program.noiDungBaiHocs.splice(targetIndex, 0, movedLesson);

    this.program.noiDungBaiHocs.forEach((lesson, i) => {
      lesson.soThuTu = i + 1;
    });

    this.draggedLessonIndex = null;
  }

  /** ✅ Lưu file vào danh sách bài học (Không upload ngay) */
  addFileToLesson(file: File, lessonIndex: number) {
    const allowedTypes = ['application/pdf', 'application/msword'
    ,'application/vnd.openxmlformats-officedocument.wordprocessingml.document'];
    const maxSizeInBytes = 10 * 1024 * 1024; // 10MB
    if (!allowedTypes.includes(file.type)) {
      alert('Chỉ chấp nhận file .pdf, .doc, .docx !');
      return;
    }
    if (file.size > maxSizeInBytes) {
      alert('File vượt quá 10MB. Vui lòng chọn file nhỏ hơn.');
      return;
    }
    this.program.noiDungBaiHocs[lessonIndex].taiLieuHocTaps.push({
      urlType: file.type.includes('pdf') ? 'pdf' : 'word',
      file
    });
  }

  /** ✅ Xóa file khỏi danh sách */
  removeFile(lessonIndex: number, fileIndex: number) {
    this.program.noiDungBaiHocs[lessonIndex].taiLieuHocTaps.splice(fileIndex, 1);
  }

  /** ✅ Gửi chương trình lên API (bao gồm cả file) */
  saveProgram() {
    if (!this.isFormValid()) {
      this.toastr.warning('Vui lòng điền đầy đủ thông tin!');
      return;
    }
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
    this.spinner.show();
    this.chuongtrinhService.addProgram(formData).subscribe({
      next: (response) => {
        this.spinner.hide();
        if(!response.isError){
          this.toastr.success(response.message);
          this.router.navigate(['/chuongtrinh']);
        }else{
          this.toastr.error(response.message);
        }
      },
      error: (error) => {
        this.spinner.hide();
        this.toastr.warning('Lỗi khi thêm chương trình!');
      }
    });
  }

  /** ✅ Hủy thêm */
  cancel() {
    this.router.navigate(['/chuongtrinh']);
  }
}
