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
  ) {}

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

        console.log("📌 Dữ liệu sau khi gán vào `program`:", this.program);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error("❌ Lỗi khi tải chương trình học:", err);
      }
    });
  }

  /** ✅ Thêm bài học mới */
  addLesson() {
    const newLesson = {
      id: "0",
      tieuDe: '',
      mota: '',
      soThuTu: this.program.noiDungBaiHocs.length + 1, // Mặc định số thứ tự mới
      taiLieuHocTaps: [],
      expanded: false
    };

    this.program.noiDungBaiHocs.push(newLesson);
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

  /** ✅ Gửi chương trình đã chỉnh sửa lên API */
  saveProgram() {
    if (!this.program.tieuDe || !this.program.moTa) {
      alert("⚠️ Vui lòng nhập đầy đủ thông tin!");
      return;
    }

    const formData = new FormData();
    formData.append('id', this.program.id.toString());
    formData.append('tieuDe', this.program.tieuDe);
    formData.append('moTa', this.program.moTa);
    formData.append('trangThai', this.program.trangThai ?? "Đang cập nhật");

    this.program.noiDungBaiHocs.forEach((lesson, index) => {
      formData.append(`noiDungBaiHocs[${index}][id]`, lesson.id ?? "0");
      formData.append(`noiDungBaiHocs[${index}][tieuDe]`, lesson.tieuDe);
      formData.append(`noiDungBaiHocs[${index}][mota]`, lesson.mota);
      formData.append(`noiDungBaiHocs[${index}][soThuTu]`, lesson.soThuTu.toString());

      lesson.taiLieuHocTaps.forEach((file, fileIndex) => {
        formData.append(`noiDungBaiHocs[${index}][taiLieuHocTaps][${fileIndex}][id]`, file.id ?? "0");
        formData.append(`noiDungBaiHocs[${index}][taiLieuHocTaps][${fileIndex}][urlType]`, file.urlType);
        if (file.file instanceof File) {
          formData.append(`noiDungBaiHocs[${index}][taiLieuHocTaps][${fileIndex}][file]`, file.file);
        }
      });
    });

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
