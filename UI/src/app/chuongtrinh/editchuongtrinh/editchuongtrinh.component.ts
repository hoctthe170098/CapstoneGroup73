import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ChuongtrinhService } from '../shared/chuongtrinh.service';

@Component({
  selector: 'app-editchuongtrinh',
  templateUrl: './editchuongtrinh.component.html',
  styleUrls: ['./editchuongtrinh.component.scss']
})
export class EditchuongtrinhComponent implements OnInit {
  programId: number | null = null;
  program: any = {
    id: 0,
    tieuDe: '',
    moTa: '',
    noiDungBaiHocs: []
  };

  draggedLessonIndex: number | null = null; // Lưu vị trí bài học đang kéo

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private chuongtrinhService: ChuongtrinhService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.programId = Number(this.route.snapshot.paramMap.get('id'));
    console.log("📌 ID lấy từ route:", this.programId);

    if (!this.programId || isNaN(this.programId)) {
      console.error("❌ Lỗi: ID chương trình không hợp lệ!");
      return;
    }

    this.chuongtrinhService.getProgramById(this.programId).subscribe({
      next: (response) => {
        console.log("📌 Dữ liệu API trả về:", response);
        if (!response) {
          console.error("❌ API không trả về dữ liệu!");
          return;
        }
        // Gán dữ liệu vào program
        this.program = {
          id: response.id || 0,
          tieuDe: response.tieuDe || "Chưa có tiêu đề",
          moTa: response.moTa || "Chưa có mô tả",
          noiDungBaiHocs: response.noiDungBaiHocs?.map((lesson: any, index: number) => ({
            id: lesson.id || `lesson-${index}`,
            tieuDe: lesson.tieuDe || `Bài học ${index + 1}`,
            mota: lesson.mota || "Chưa có mô tả bài học",
            soThuTu: lesson.soThuTu ?? index + 1,
            taiLieuHocTaps: lesson.taiLieuHocTaps ?? [],
            expanded: false
          })) ?? []
        };
        // Sắp xếp lại noiDungBaiHocs theo soThuTu
        if (this.program.noiDungBaiHocs) {
          this.program.noiDungBaiHocs.sort((a, b) => a.soThuTu - b.soThuTu);
        }
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error("❌ Lỗi khi tải chương trình học:", err);
      }
    });
  }

  /** ✅ Thêm bài học mới */
  addLesson() {
    this.program.noiDungBaiHocs.push({
      id: null,
      tieuDe: '',
      mota: '',
      soThuTu: this.program.noiDungBaiHocs.length + 1, // Mặc định số thứ tự mới
      taiLieuHocTaps: [],
      expanded: false
    });
  }

  /** ✅ Toggle mở rộng bài học */
  toggleLesson(index: number) {
    this.program.noiDungBaiHocs[index].expanded = !this.program.noiDungBaiHocs[index].expanded;
  }

  /** ✅ Xóa bài học */
  removeLesson(index: number) {
    this.program.noiDungBaiHocs.splice(index, 1);
    this.program.noiDungBaiHocs.forEach((lesson, i) => {
      lesson.soThuTu = i + 1;
    });
  }

  /** ✅ Lưu lại vị trí bài học đang kéo */
  onDragStart(event: DragEvent, index: number) {
    this.draggedLessonIndex = index;
    event.dataTransfer?.setData("text/plain", index.toString());
  }

  /** ✅ Xử lý khi kéo qua phần tử khác */
  onDragOver(event: DragEvent) {
    event.preventDefault();
  }

  /** ✅ Xử lý khi thả bài học */
  onDropLesson(event: DragEvent, targetIndex: number) {
    event.preventDefault();
    if (this.draggedLessonIndex === null || this.draggedLessonIndex === targetIndex) return;

    // Hoán đổi vị trí giữa bài học được kéo và bài học mục tiêu
    const movedLesson = this.program.noiDungBaiHocs[this.draggedLessonIndex];
    this.program.noiDungBaiHocs.splice(this.draggedLessonIndex, 1);
    this.program.noiDungBaiHocs.splice(targetIndex, 0, movedLesson);
    // Cập nhật số thứ tự cho tất cả bài học
    this.program.noiDungBaiHocs.forEach((lesson, i) => {
      lesson.soThuTu = i + 1;
    });
    // Reset chỉ số bài học đang kéo
    this.draggedLessonIndex = null;
  }
  /** ✅ Upload file vào danh sách */
  uploadFile(file: File, lessonIndex: number) {
    const allowedTypes = ['application/pdf', 'application/msword', 'video/mp4'];
    if (!allowedTypes.includes(file.type)) {
      alert('Chỉ chấp nhận file PDF, Word, MP4!');
      return;
    }
    this.program.noiDungBaiHocs[lessonIndex].taiLieuHocTaps.push({
      urlType: file.type.includes('video') ? 'video' : 'pdf',
      file,
      ten: file.name
    });
  }
  /** ✅ Gửi chương trình đã chỉnh sửa lên API */
  saveProgram() {
    console.log(this.program)
    if (!this.program.tieuDe || !this.program.moTa) {
      alert("⚠️ Vui lòng nhập đầy đủ thông tin!");
      return;
    }

    const formData = new FormData();
    formData.append('chuongTrinhDto.id', this.program.id);
    formData.append('chuongTrinhDto.tieuDe', this.program.tieuDe);
    formData.append('chuongTrinhDto.moTa', this.program.moTa);
    // Lặp qua danh sách bài học và thêm vào FormData
    this.program.noiDungBaiHocs.forEach((lesson, index) => {
      if (lesson.id !== null && lesson.id !== undefined) {
        formData.append(`chuongTrinhDto.noiDungBaiHocs[${index}].id`, lesson.id.toString());
      }
      formData.append(`chuongTrinhDto.noiDungBaiHocs[${index}].tieuDe`, lesson.tieuDe);
      formData.append(`chuongTrinhDto.noiDungBaiHocs[${index}].moTa`, lesson.mota);
      formData.append(`chuongTrinhDto.noiDungBaiHocs[${index}].soThuTu`, lesson.soThuTu);
      // Lặp qua danh sách tài liệu của từng bài học
      lesson.taiLieuHocTaps.forEach((file, fileIndex) => {
        if (file.id !== null && file.id !== undefined) {
          formData.append(`chuongTrinhDto.noiDungBaiHocs[${index}].taiLieuHocTaps[${fileIndex}].id`, file.id.toString());
        }
        formData.append(`chuongTrinhDto.noiDungBaiHocs[${index}].taiLieuHocTaps[${fileIndex}].urlType`, file.urlType);
        formData.append(`chuongTrinhDto.noiDungBaiHocs[${index}].taiLieuHocTaps[${fileIndex}].file`, file.file);
      });
    });
    console.log("📌 Dữ liệu gửi lên API (FormData):", this.program);

    // Gọi API cập nhật chương trình
    this.chuongtrinhService.updateProgram(formData).subscribe({
      next: (response) => {
        if (response.isError) {
          alert(`❌ Lỗi: ${response.message || "Có lỗi xảy ra!"}`);
        } else {
          alert("✅ Cập nhật chương trình thành công!");
          this.router.navigate(['/chuongtrinh']);
        }
      },
      error: (error) => {
        alert(`❌ Lỗi khi cập nhật: ${error?.error?.message || "Lỗi không xác định!"}`);
      }
    });
  }

  /** ✅ Hủy chỉnh sửa */
  cancelEdit() {
    this.router.navigate(['/chuongtrinh']);
  }
}
