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
    
        // Kiểm tra nếu response hoặc response.data không tồn tại
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
    
        console.log("📌 Dữ liệu sau khi gán vào `program`:", this.program);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error("❌ Lỗi khi tải chương trình học:", err);
      }
    });
  }

  /** ✅ Thêm bài học */
  addLesson() {
    this.program.noiDungBaiHocs.push({
      id: "0",
      tieuDe: '',
      mota: '',
      soThuTu: this.program.noiDungBaiHocs.length + 1,
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

  /** ✅ Xử lý tải file lên */
  onFileChange(event: Event, lessonIndex: number) {
    const files = (event.target as HTMLInputElement).files;
    if (files) {
      for (let i = 0; i < files.length; i++) {
        this.uploadFile(files[i], lessonIndex);
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
        this.uploadFile(event.dataTransfer.files[i], lessonIndex);
      }
    }
  }

  /** ✅ Upload file vào danh sách */
  uploadFile(file: File, lessonIndex: number) {
    const allowedTypes = ['application/pdf', 'application/msword', 'video/mp4'];
    if (!allowedTypes.includes(file.type)) {
      alert('Chỉ chấp nhận file PDF, Word, MP4!');
      return;
    }

    this.chuongtrinhService.uploadFile(file).subscribe(
      fileUrl => {
        if (!this.program.noiDungBaiHocs[lessonIndex].taiLieuHocTaps) {
          this.program.noiDungBaiHocs[lessonIndex].taiLieuHocTaps = [];
        }
        this.program.noiDungBaiHocs[lessonIndex].taiLieuHocTaps.push({
          id: "0",
          urlType: file.type,
          file: fileUrl
        });
      },
      error => {
        console.error('❌ Lỗi upload file:', error);
        alert('Upload file thất bại!');
      }
    );
  }

  /** ✅ Xóa file khỏi danh sách */
  removeFile(lessonIndex: number, fileIndex: number) {
    this.program.noiDungBaiHocs[lessonIndex].taiLieuHocTaps.splice(fileIndex, 1);
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
  
    // Lặp qua danh sách bài học và thêm vào FormData
    this.program.noiDungBaiHocs.forEach((lesson, index) => {
      formData.append(`noiDungBaiHocs[${index}][id]`, lesson.id ?? "0");
      formData.append(`noiDungBaiHocs[${index}][tieuDe]`, lesson.tieuDe);
      formData.append(`noiDungBaiHocs[${index}][mota]`, lesson.mota);
      formData.append(`noiDungBaiHocs[${index}][soThuTu]`, lesson.soThuTu.toString());
  
      // Lặp qua danh sách tài liệu của từng bài học
      lesson.taiLieuHocTaps.forEach((file, fileIndex) => {
        formData.append(`noiDungBaiHocs[${index}][taiLieuHocTaps][${fileIndex}][id]`, file.id ?? "0");
        formData.append(`noiDungBaiHocs[${index}][taiLieuHocTaps][${fileIndex}][urlType]`, file.urlType);
  
        // Nếu `file.file` là một đối tượng `File`, thêm vào FormData
        if (file.file instanceof File) {
          formData.append(`noiDungBaiHocs[${index}][taiLieuHocTaps][${fileIndex}][file]`, file.file);
        }
      });
    });
  
    console.log("📌 Dữ liệu gửi lên API (FormData):", formData);
  
    // Gọi API cập nhật chương trình
    this.chuongtrinhService.updateProgram(formData).subscribe({
      next: (response) => {
        if (response.isError) {
          console.error("⚠️ API trả về lỗi:", response);
          alert(`❌ Lỗi: ${response.message || "Có lỗi xảy ra!"}`);
        } else {
          alert("✅ Cập nhật chương trình thành công!");
          this.router.navigate(['/chuongtrinh']); // Điều hướng về danh sách
        }
      },
      error: (error) => {
        console.error("❌ Lỗi khi gọi API:", error);
    
        let errorMessage = "Lỗi không xác định!";
        if (error?.error?.isError) {
          errorMessage = error.error.message;
        }
    
        alert(`❌ Lỗi khi cập nhật: ${errorMessage}`);
      }
    });
  }
  

  /** ✅ Hủy chỉnh sửa */
  cancelEdit() {
    this.router.navigate(['/chuongtrinh']);
  }
}
